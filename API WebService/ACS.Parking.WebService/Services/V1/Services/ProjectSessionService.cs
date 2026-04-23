using ACS.Background;
using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Cache.Service.V1.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ACS.Database.IDataAccess.ParkingService.V1;
using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Models.Response.V1.ParkingService.Session;
using ACS.Parking.WebService.Services.V1.Interfaces;

namespace ACS.Parking.WebService.Services.V1.Services
{
    public class ProjectSessionService(ILicenseManager licenseManager, IRedisCacheService redisCacheService, IBaseService baseService, IBackgroundTaskQueue backgroundTaskQueue,  ILogger<ProjectSessionService> logger) : Service.Service(licenseManager), IProjectSessionService
    {
        private const string SessionKeyFormat = "parking:session:ws{0}:proj{1}:status:{2}:id:{3}";
        private const string SessionIdsKeyFormat = "parking:sessions:ids:ws{0}:proj{1}:status:{2}";
        private const string TotalCountKeyFormat = "parking:sessions:count:ws{0}:proj{1}:status:{2}";



        private IProjectSessionAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.ParkingService.V1.ProjectSessionAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in SessionService.")
                };
            }
        }

        public async Task<SessionsResponse> GetProjectSessionsForBatchAsync(
            int workspace,
            int project,
            int skip,
            int take,
            string? status,
            CancellationToken cancellationToken = default) =>
            await this.GetSessionsPageAsync(
                workspace: workspace,
                project: project,
                skip: skip,
                take: take,
                status: status,
                getDatabaseSessions: async (dbSkip, dbTake) =>
                {
                    var license = this.LicenseManager.GetLicense();
                    var response =  await this[license?.DB!].GetParkingSessionsByProjectAsync(
                        workspace.ToString(),
                        project.ToString(),
                        skip.ToString(),
                        take.ToString(),
                        status,
                        cancellationToken);

                    return response.Sessions?
                    .Select(s => baseService.MapSessionDataToResponse(s))
                    .Where(s => s != null)
                    .Select(s => s!) 
                    .ToList()
                    ?? [];
                },
                 cancellationToken: cancellationToken);

        /// <summary>
        /// Adds a new session to the cache, managing pagination automatically.
        /// Updates the first page by inserting the new session at the beginning,
        /// and invalidates other pages to maintain consistency.
        /// </summary>
        /// <param name="workspace">Workspace ID</param>
        /// <param name="project">Project ID</param>
        /// <param name="newSession">The new session to add</param>
        /// <param name="status">Optional status filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task AddSessionToCacheAsync(
            int workspace,
            int project,
            ParkingSessionDataResponse session,
            string? status = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var sessionKey = BuildSessionKey(workspace, project, status, session.ParkingSession?.ParkingSessionId.ToString()!);
                await redisCacheService.SetAsync(
                    sessionKey,
                    session,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    },
                    cancellationToken);


                // Add session ID to the list (at the beginning - newest first)
                await AddSessionIdToListAsync(workspace, project, status, session.ParkingSession?.ParkingSessionId.ToString()!, cancellationToken);

                // Increment total count
                await IncrementTotalCountAsync(workspace, project, status, cancellationToken);

                logger?.LogInformation(
                    "Added session {SessionId} to cache for ws:{Workspace}, proj:{Project}, status:{Status}",
                    session.ParkingSession?.ParkingSessionId.ToString(), workspace, project, status ?? "all");

            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error adding session to cache for workspace {Workspace}, project {Project}",
                    workspace, project);
            }
        }

        /// <summary>
        /// Adds session ID to the beginning of the IDs list
        /// </summary>
        public async Task AddSessionIdToListAsync(
            int workspace,
            int project,
            string? status,
            string sessionId,
            CancellationToken cancellationToken)
        {
            var idsKey = BuildSessionIdsKey(workspace, project, status);
            // Get current list
            var sessionIds = await redisCacheService.TryGetAsync<List<string>>(idsKey, cancellationToken) ?? [];

            // Add new ID at the beginning (newest first)
            if (!sessionIds.Contains(sessionId))
            {
                sessionIds.Insert(0, sessionId);
                // Keep only last 1000 IDs to prevent unlimited growth
                if (sessionIds.Count > 1000) sessionIds = [.. sessionIds.Take(1000)];

                await redisCacheService.SetAsync(
                    idsKey,
                    sessionIds,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    },
                    cancellationToken);
            }
        }


        public async Task CloseOldSessionsInCacheAsync(
           int workspace,
           int project,
           string plateCode,
           string plateNumber,
           int country,
           int state,
           int category,
           string excludeSessionId,
           CancellationToken cancellationToken)
        {
            try
            {
                var sessionIds = await GetSessionIdsFromCacheAsync(workspace, project,  "IN", cancellationToken);
                if (sessionIds == null || sessionIds.Count == 0) return;
                var updatedSessions = new List<string>();
                var sessionsToMove = new List<ParkingSessionDataResponse>();
                foreach (var sessionId in sessionIds)
                {
                    if (sessionId == excludeSessionId)
                    {
                        updatedSessions.Add(sessionId);
                        continue;
                    }
                    var sessionKey = BuildSessionKey(workspace, project,  "IN", sessionId);
                    var session = await redisCacheService.TryGetAsync<ParkingSessionDataResponse>(sessionKey, cancellationToken);
                    if (session != null && session.ParkingSession != null)
                    {
                        bool isMatchingPlate = (session.ParkingSession.EntryPlateCode?.ToUpper() ?? "").Equals(plateCode, StringComparison.CurrentCultureIgnoreCase) &&
                            (session.ParkingSession.EntryPlateNumber?.ToUpper() ?? "").Equals(plateNumber, StringComparison.CurrentCultureIgnoreCase) &&
                            session?.Country?.CountryId == country &&
                            session.PlateState?.PlateStateId == state &&
                            session.PlateState.CountryId == country &&
                            session.PlateCategory?.PlateCategoryId == category;
                        if (isMatchingPlate && session != null)
                        {
                            session.ParkingSession.Status = "CANCELLED";
                            session.ParkingSession.UpdatedAt = DateTime.UtcNow;
                            await redisCacheService.RemoveAsync(sessionKey, cancellationToken);
                            sessionsToMove.Add(session);
                            logger.LogInformation(
                                "Closed old session {SessionId} in cache for plate {PlateNumber}",
                                sessionId,
                                plateNumber);
                        }
                        else
                        {
                            updatedSessions.Add(sessionId);
                        }
                    }
                    else
                    {
                        updatedSessions.Add(sessionId);
                    }
                }

                if (updatedSessions.Count != sessionIds.Count)
                {
                    var idsKey = BuildSessionIdsKey(workspace, project,  "IN");
                    await redisCacheService.SetAsync(
                        idsKey,
                        updatedSessions,
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                        },
                        cancellationToken);

                    var removedCount = sessionIds.Count - updatedSessions.Count;
                    await DecrementTotalCountAsync(workspace, project, "IN", removedCount, cancellationToken);
                }

                foreach (var closedSession in sessionsToMove)
                {
                    await AddSessionToCacheAsync(
                        workspace,
                        project,
                        closedSession,
                        status: "CANCELLED",
                        cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error closing old sessions in cache");
            }
        }


        private async Task DecrementTotalCountAsync(
           int workspace,
           int project,
           string? status,
           int decrementBy,
           CancellationToken cancellationToken)
        {
            var countKey = BuildTotalCountKey(workspace, project, status);
            var currentCount = await redisCacheService.TryGetAsync<int>(countKey, cancellationToken);
            var newCount = Math.Max(0, currentCount - decrementBy);

            await redisCacheService.SetAsync(
                countKey,
                newCount,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                },
                cancellationToken);
        }

        /// <summary>
        /// Increments the total count
        /// </summary>
        public async Task IncrementTotalCountAsync(
            int workspace,
            int project,
            string? status,
            CancellationToken cancellationToken)
        {
            var countKey = BuildTotalCountKey(workspace, project, status);

            var currentCount = await redisCacheService.TryGetAsync<int>(countKey, cancellationToken);
            var newCount = currentCount + 1;

            await redisCacheService.SetAsync(
                countKey,
                newCount,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                },
                cancellationToken);
        }

        /// <summary>
        /// Gets a page of sessions by combining Redis cache + Database
        /// </summary>
        public async Task<SessionsResponse> GetSessionsPageAsync(
            int workspace,
            int project,
            int skip,
            int take,
            string? status,
            Func<int, int, Task<List<ParkingSessionDataResponse>>> getDatabaseSessions,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var sessionIds = await GetSessionIdsFromCacheAsync(workspace, project, status, cancellationToken);
                List<ParkingSessionDataResponse> cachedSessions = [];
                if (sessionIds != null && sessionIds.Count > 0)
                {
                    var pageSessionIds = sessionIds.Skip(skip).Take(take).ToList();
                    foreach (var sessionId in pageSessionIds)
                    {
                        var sessionKey = BuildSessionKey(workspace, project, status, sessionId);
                        var session = await redisCacheService.TryGetAsync<ParkingSessionDataResponse>(sessionKey, cancellationToken);
                        if (session != null) cachedSessions.Add(session);                        
                    }
                }
                var cachedCount = cachedSessions.Count;
                var remainingNeeded = take - cachedCount;
                List<ParkingSessionDataResponse> allSessions = [.. cachedSessions];
                if (remainingNeeded > 0)
                {
                    logger?.LogDebug("Cache has {CachedCount} sessions, need {RemainingNeeded} more from DB",cachedCount, remainingNeeded);
                    var dbSkip = skip + cachedCount;
                    var dbSessions = await getDatabaseSessions(dbSkip, remainingNeeded);
                    if (dbSessions != null && dbSessions.Count > 0)
                    {
                        allSessions.AddRange(dbSessions);
                        foreach (var dbSession in dbSessions)
                        {
                            backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
                            {
                                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, cancellationToken);
                                await AddSessionToCacheAsync(workspace, project, dbSession, status, linkedCts.Token);
                            });
                        }
                    }
                }
                var totalCount = await GetTotalCountAsync(workspace, project, status, cancellationToken);
                if (totalCount == 0 && allSessions.Count > 0) totalCount = skip + allSessions.Count + 1;
                return new SessionsResponse
                {
                    Success = true,
                    Sessions = allSessions,
                    Pagination = new PaginationInfoResponse
                    {
                        Total = totalCount,
                        Skip = skip,
                        Take = take,
                        HasMore = allSessions.Count == take 
                    }
                };
            }
            catch(Exception ex)
            {
                logger?.LogError(ex, "Error getting sessions page");
                var dbSessions = await getDatabaseSessions(skip, take);
                return new SessionsResponse
                {
                    Success = true,
                    Sessions = dbSessions,
                    Pagination = new PaginationInfoResponse
                    {
                        Total = skip + dbSessions.Count,
                        Skip = skip,
                        Take = take,
                        HasMore = dbSessions.Count == take
                    }
                };
            }
        }


        /// <summary>
        /// Gets the total count from cache
        /// </summary>
        private async Task<int> GetTotalCountAsync(
            int workspace,
            int project,
            string? status,
            CancellationToken cancellationToken)
        {
            var countKey = BuildTotalCountKey(workspace, project, status);
            return await redisCacheService.TryGetAsync<int>(countKey, cancellationToken);
        }

        /// <summary>
        /// Gets the list of session IDs from cache
        /// </summary>
        private async Task<List<string>?> GetSessionIdsFromCacheAsync(
            int workspace,
            int project,
            string? status,
            CancellationToken cancellationToken)
        {
            var idsKey = BuildSessionIdsKey(workspace, project, status);
            return await redisCacheService.TryGetAsync<List<string>>(idsKey, cancellationToken);
        }

        private static string BuildSessionKey(int workspace, int project, string? status, string sessionId) => string.Format(SessionKeyFormat, workspace, project, status ?? "*", sessionId);
        private static string BuildSessionIdsKey(int workspace, int project, string? status) => string.Format(SessionIdsKeyFormat, workspace, project, status ?? "*");
        private static string BuildTotalCountKey(int workspace, int project, string? status) => string.Format(TotalCountKeyFormat, workspace, project, status ?? "*");


    }
}

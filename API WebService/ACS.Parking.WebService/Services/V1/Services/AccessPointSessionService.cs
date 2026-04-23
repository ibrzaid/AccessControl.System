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
    public class AccessPointSessionService(ILicenseManager licenseManager, IRedisCacheService redisCacheService, IBaseService baseService, IBackgroundTaskQueue backgroundTaskQueue, ILogger<AccessPointSessionService> logger) : Service.Service(licenseManager), IAccessPointSessionService
    {
        private const string SessionKeyFormat = "parking:session:ws{0}:proj{1}:area{2}:zone{3}access{4}:status:{5}:id:{6}";
        private const string SessionIdsKeyFormat = "parking:sessions:ids:ws{0}:proj{1}:area{2}:zone{3}access{4}:status:{5}";
        private const string TotalCountKeyFormat = "parking:sessions:count:ws{0}:proj{1}:area{2}:zone{3}access{4}:status:{5}";



        private IAccessPoinSesssionDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.ParkingService.V1.AccessPoinSesssionDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in SessionService.")
                };
            }
        }


        public async Task<SessionsResponse> GetParkingSessionsByAccessPointAsync(
            int workspace,
            int project,
            int projectArea,
            int zone,
            int accessPoint,
            int skip,
            int take,
            string? status = null,
            CancellationToken cancellationToken = default) =>
             await this.GetSessionsPageAsync(
                  workspace: workspace,
                  project: project,
                  projectArea: projectArea,
                  zone: zone,
                  accessPoint: accessPoint,
                  status: status,
                  skip: skip,
                  take: take,
                  getDatabaseSessions: async (dbSkip, dbTake) =>
                  {
                      var license = this.LicenseManager.GetLicense();
                      var response = await this[license?.DB!].GetParkingSessionsByAccessPointAsync(
                            workspace.ToString(),
                            project.ToString(),
                            projectArea.ToString(),
                            zone.ToString(),
                            accessPoint.ToString(),                           
                            dbSkip.ToString(),
                            dbTake.ToString(),
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

        public async Task AddSessionToCacheAsync(int workspace, int project, int projectArea, int zone, int accessPoint, ParkingSessionDataResponse session, string? status = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var sessionKey = BuildSessionKey(workspace, project, projectArea, zone, accessPoint, status, session.ParkingSession?.ParkingSessionId.ToString()!);
                await redisCacheService.SetAsync(
                    sessionKey,
                    session,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    },
                    cancellationToken);
                // Add session ID to the list (at the beginning - newest first)
                await AddSessionIdToListAsync(workspace, project, projectArea, zone, accessPoint,  status, session.ParkingSession?.ParkingSessionId.ToString()!, cancellationToken);
                // Increment total count
                await IncrementTotalCountAsync(workspace, project, projectArea, zone, accessPoint, status, cancellationToken);

                logger.LogInformation(
                    "Added session to cache: Workspace {Workspace}, Project {Project}, ProjectArea {ProjectArea}, Zone {Zone}, AccessPoint {AccessPoint}",
                    workspace, project, projectArea, zone, accessPoint);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, 
                    "Error adding session to cache: Workspace {Workspace}, Project {Project}, ProjectArea {ProjectArea}, Zone {Zone}, AccessPoint {AccessPoint}", 
                    workspace, project, projectArea, zone, accessPoint);
            }
        }


        public async Task CloseOldSessionsInCacheAsync(
           int workspace,
           int project,
           int projectArea,
           int zone,
           int accessPoint,
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
                var sessionIds = await GetSessionIdsFromCacheAsync(workspace, project, projectArea, zone, accessPoint,"IN", cancellationToken);
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
                    var sessionKey = BuildSessionKey(workspace, project, projectArea, zone, accessPoint, "IN", sessionId);
                    var session = await redisCacheService.TryGetAsync<ParkingSessionDataResponse>(sessionKey, cancellationToken);
                    if (session != null && session.ParkingSession != null)
                    {
                        bool isMatchingPlate = (session.ParkingSession.EntryPlateCode?.ToUpper() ?? "").Equals(plateCode, StringComparison.CurrentCultureIgnoreCase) && 
                            (session.ParkingSession.EntryPlateNumber?.ToUpper() ?? "").Equals(plateNumber, StringComparison.CurrentCultureIgnoreCase) &&
                            session?.Country?.CountryId == country &&
                            session.PlateState?.PlateStateId == state &&
                            session.PlateState.CountryId== country &&
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
                    var idsKey = BuildSessionIdsKey(workspace, project, projectArea, zone, accessPoint, "IN");
                    await redisCacheService.SetAsync(
                        idsKey,
                        updatedSessions,
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                        },
                        cancellationToken);

                    var removedCount = sessionIds.Count - updatedSessions.Count;
                    await DecrementTotalCountAsync(workspace, project, projectArea, zone, accessPoint, "IN", removedCount, cancellationToken);
                }

                foreach (var closedSession in sessionsToMove)
                {
                    await AddSessionToCacheAsync(
                        workspace,
                        project,
                        projectArea,
                        zone,
                        accessPoint,
                        closedSession,
                        status: "CANCELLED",
                        cancellationToken);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error closing old sessions in cache");
            }
        }

        private async Task DecrementTotalCountAsync(
           int workspace,
           int project,
           int projectArea,
           int zone,
           int accessPoint,
           string? status,
           int decrementBy,
           CancellationToken cancellationToken)
        {
            var countKey = BuildTotalCountKey(workspace, project, projectArea, zone, accessPoint, status);
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


        public async Task AddSessionIdToListAsync(
           int workspace,
           int project,
           int projectArea,
           int zone,
           int accessPoint,
           string? status,
           string sessionId,
           CancellationToken cancellationToken)
        {
            var idsKey = BuildSessionIdsKey(workspace, project, projectArea, zone, accessPoint, status);
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

        public async Task IncrementTotalCountAsync(
            int workspace, 
            int project, 
            int projectArea, 
            int zone, 
            int accessPoint, 
            string? status, 
            CancellationToken cancellationToken)
        {
            var countKey = BuildTotalCountKey(workspace, project, projectArea, zone, accessPoint,  status);
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


        public async Task<SessionsResponse> GetSessionsPageAsync(
            int workspace, 
            int project, 
            int projectArea, 
            int zone, 
            int accessPoint, 
            int skip, 
            int take, 
            string? status, 
            Func<int, int, Task<List<ParkingSessionDataResponse>>> getDatabaseSessions, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var sessionIds = await GetSessionIdsFromCacheAsync(workspace, project, projectArea, zone, accessPoint, status, cancellationToken);
                List<ParkingSessionDataResponse> cachedSessions = [];
                if (sessionIds != null && sessionIds.Count > 0)
                {
                    var pageSessionIds = sessionIds.Skip(skip).Take(take).ToList();
                    foreach (var sessionId in pageSessionIds)
                    {
                        var sessionKey = BuildSessionKey(workspace, project, projectArea, zone, accessPoint, status, sessionId);
                        var session = await redisCacheService.TryGetAsync<ParkingSessionDataResponse>(sessionKey, cancellationToken);
                        if (session != null) cachedSessions.Add(session);
                    }
                }
                var cachedCount = cachedSessions.Count;
                var remainingNeeded = take - cachedCount;
                List<ParkingSessionDataResponse> allSessions = [.. cachedSessions];
                if (remainingNeeded > 0)
                {
                    logger?.LogDebug("Cache has {CachedCount} sessions, need {RemainingNeeded} more from DB", cachedCount, remainingNeeded);
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
                                await AddSessionToCacheAsync(workspace, project, projectArea, zone, accessPoint, dbSession, status, linkedCts.Token);
                            });
                        }
                    }
                }
                var totalCount = await GetTotalCountAsync(workspace, project, projectArea, zone, accessPoint, status, cancellationToken);
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
            catch (Exception ex)
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

        private async Task<List<string>?> GetSessionIdsFromCacheAsync(
            int workspace, 
            int project, 
            int projectArea, 
            int zone, 
            int accessPoint, 
            string? status, 
            CancellationToken cancellationToken)
        {
            var idsKey = BuildSessionIdsKey(workspace, project, projectArea, zone, accessPoint, status);
            return await redisCacheService.TryGetAsync<List<string>>(idsKey, cancellationToken);
        }

        private async Task<int> GetTotalCountAsync(
            int workspace, 
            int project, 
            int projectArea, 
            int zone, 
            int accessPoint, 
            string? status, 
            CancellationToken cancellationToken)
        {
            var countKey = BuildTotalCountKey(workspace, project, projectArea, zone, accessPoint, status);
            return await redisCacheService.TryGetAsync<int>(countKey, cancellationToken);
        }

        private static string BuildSessionKey(int workspace, int project, int projectArea, int zone, int accessPoint, string? status, string sessionId) => string.Format(SessionKeyFormat, workspace, project, projectArea, zone, accessPoint, status ?? "*", sessionId);
        private static string BuildSessionIdsKey(int workspace, int project, int projectArea, int zone, int accessPoint, string? status) => string.Format(SessionIdsKeyFormat, workspace, project, projectArea, zone, accessPoint, status ?? "*");
        private static string BuildTotalCountKey(int workspace, int project, int projectArea, int zone, int accessPoint, string? status) => string.Format(TotalCountKeyFormat, workspace, project, projectArea, zone, accessPoint, status ?? "*");
    }
}

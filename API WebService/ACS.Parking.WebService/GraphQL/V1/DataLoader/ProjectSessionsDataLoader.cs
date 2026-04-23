
using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Models.Response.V1.ParkingService.Session;
using ACS.Parking.WebService.Services.V1.Interfaces;
using GraphQL.DataLoader;

namespace ACS.Parking.WebService.GraphQL.V1.DataLoader
{


    public class ProjectSessionKeyComparer
    : IEqualityComparer<(int workspace, int project, int skip, int take, string? status)>
    {
        public bool Equals(
            (int, int, int, int, string?) x,
            (int, int, int, int, string?) y)
            => x == y;

        public int GetHashCode((int workspace, int project, int skip, int take, string? status) obj)
            => HashCode.Combine(obj.workspace, obj.project, obj.skip, obj.take, obj.status);
    }

    public class ProjectSessionsDataLoader(
        IProjectSessionService sessionService,
        ILogger<ProjectSessionsDataLoader> logger,
        IEqualityComparer<(int, int, int, int, string?)>? keyComparer = null,
        int maxBatchSize = 100) : BatchDataLoader<(int, int, int, int, string?), SessionsResponse>(
            fetchDelegate: async (keys, cancellationToken) =>
            {
                var result = new Dictionary<(int, int, int, int, string?), SessionsResponse>();
                try
                {
                    foreach (var key in keys)
                    {
                        var (workspaceId, projectId, skip, take, status) = key;
                        try
                        {
                            var response = await sessionService.GetProjectSessionsForBatchAsync(
                                workspaceId,
                                projectId,
                                skip,
                                take,
                                status,
                                cancellationToken);
                            result[key] = response ?? new()
                            {
                                Success = false,
                                Error = "No response received"
                            };

                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "No response returned for workspace {WorkspaceId}, project {ProjectId}", workspaceId, projectId);
                            result[key] = new SessionsResponse
                            {
                                Success = false,
                                Error = $"Something went wrong. Please try again later.",
                                ErrorCode = "INTERNAL_ERROR"
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in loading Parking Session batch loader");
                }
                return result;
               
            },
            keyComparer: keyComparer?? new ProjectSessionKeyComparer(),
            defaultValue: new SessionsResponse { Success = false, Error = "Default error" },
            maxBatchSize: maxBatchSize
            )
    {
        public Task<SessionsResponse> LoadSingleAsync(
         int workspace,
         int project,
         int skip,
         int take,
         string? status,
         CancellationToken cancellationToken = default)
        {
            var key = (workspace, project, skip, take, status);
            
            return LoadAsync(key).GetResultAsync(cancellationToken);
            
        }

       
    }
}

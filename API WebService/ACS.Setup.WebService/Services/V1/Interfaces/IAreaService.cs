using ACS.Models.Request.V1.SetupService.Area;
using ACS.Models.Response.V1.SetupService.Area;

namespace ACS.Setup.WebService.Services.V1.Interfaces
{
    public interface IAreaService
    {
        Task<GetAreasResponse> GetAreasAsync(int workspaceId, int callerId, int projectId, int limit, int offset, CancellationToken ct = default);
        Task<OperationAreaResultResponse> CreateAreaAsync(int workspaceId, int callerId, CreateAreaRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationAreaResultResponse> UpdateAreaAsync(int workspaceId, int callerId, int projectAreaId, UpdateAreaRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationAreaResultResponse> DeleteAreaAsync(int workspaceId, int callerId, int projectAreaId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default);
    }
}

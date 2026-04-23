using ACS.Models.Request.V1.SetupService.AccessPoint;
using ACS.Models.Response.V1.SetupService.AccessPoint;

namespace ACS.Setup.WebService.Services.V1.Interfaces
{
    public interface IAccessPointService
    {
        Task<GetAccessPointsResponse> GetAccessPointsAsync(int workspaceId, int callerId, int zoneId, int limit, int offset, CancellationToken ct = default);
        Task<OperationAccessPointResultResponse> CreateAccessPointAsync(int workspaceId, int callerId, CreateAccessPointRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationAccessPointResultResponse> UpdateAccessPointAsync(int workspaceId, int callerId, int accessPointId, UpdateAccessPointRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationAccessPointResultResponse> DeleteAccessPointAsync(int workspaceId, int callerId, int accessPointId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default);
    }
}

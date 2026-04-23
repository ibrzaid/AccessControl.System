using ACS.Models.Request.V1.SetupService.Zone;
using ACS.Models.Response.V1.SetupService.Zone;

namespace ACS.Setup.WebService.Services.V1.Interfaces
{
    public interface IZonesService
    {
        Task<GetZonesResponse> GetZonesAsync(int workspaceId, int callerId, int projectAreaId, int limit, int offset, CancellationToken ct = default);
        Task<OperationZoneResultResponse> CreateZoneAsync(int workspaceId, int callerId, CreateZoneRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationZoneResultResponse> UpdateZoneAsync(int workspaceId, int callerId, int zoneId, UpdateZoneRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationZoneResultResponse> DeleteZoneAsync(int workspaceId, int callerId, int zoneId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default);
    }
}

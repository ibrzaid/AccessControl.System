using ACS.Models.Response;
using ACS.Models.Request.V1.SetupService.Hardware;
using ACS.Models.Response.V1.SetupService.Hardware;

namespace ACS.Setup.WebService.Services.V1.Interfaces
{
    public interface IHardwareService
    {
        Task<GetHardwareResponse> GetHardwareAsync(int workspaceId, int callerId, int accessPointId, int limit, int offset, CancellationToken ct = default);
        Task<OperationHardwareResultResponse> CreateHardwareAsync(int workspaceId, int callerId, CreateHardwareRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationHardwareResultResponse> UpdateHardwareAsync(int workspaceId, int callerId, int hardwareId, UpdateHardwareRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationHardwareResultResponse> DeleteHardwareAsync(int workspaceId, int callerId, int hardwareId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default);

        /// <summary>
        /// Connectivity / health-check probe for a hardware asset. Stub
        /// implementation returns success; real device interrogation will be
        /// wired in later.
        /// </summary>
        Task<BaseResponse> TestHardwareAsync(int workspaceId, int callerId, int? hardwareId, TestHardwareRequest req, string? ip, string? ua, string? requestId, CancellationToken ct = default);
    }
}

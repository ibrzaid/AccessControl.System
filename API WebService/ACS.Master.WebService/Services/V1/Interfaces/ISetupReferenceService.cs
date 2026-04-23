using ACS.Models.Response.V1.MasterService.SetupReference;

namespace ACS.Master.WebService.Services.V1.Interfaces
{
    public interface ISetupReferenceService
    {
        Task<GetSetupReferenceResponse> GetProjectTypesAsync(CancellationToken ct = default);
        Task<GetSetupReferenceResponse> GetAreaTypesAsync(CancellationToken ct = default);
        Task<GetSetupReferenceResponse> GetZoneTypesAsync(CancellationToken ct = default);
        Task<GetSetupReferenceResponse> GetAccessPointTypesAsync(CancellationToken ct = default);
        Task<GetSetupReferenceResponse> GetAccessLevelsAsync(CancellationToken ct = default);
        Task<GetSetupReferenceResponse> GetStatusesAsync(CancellationToken ct = default);

        Task<GetSetupReferenceResponse> GetHardwareTypesAsync(CancellationToken ct = default);
        Task<GetSetupReferenceResponse> GetHardwareStatusesAsync(CancellationToken ct = default);

        Task<GetSetupReferenceResponse> GetCountriesAsync(CancellationToken ct = default);
    }
}

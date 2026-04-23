using ACS.Models.Response.V1.MasterService.PlateState;

namespace ACS.Service.V1.Interfaces
{
    public interface IPlateStateService
    {
        Task<List<PlateStateResponse>> GetAllPlateStatesAsync(bool? isActive = null, CancellationToken cancellationToken = default);
        Task<PlateStateResponse?> GetPlateStateByIdAsync(int plateStateId, CancellationToken cancellationToken = default);
        Task<List<PlateStateResponse>> GetPlateStatesByCountryIdAsync(int countryId, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<Dictionary<int, PlateStateResponse>> GetPlateStatesByIdsBatchAsync(List<int> plateStateIds, CancellationToken cancellationToken = default);
        Task<Dictionary<int, List<PlateStateResponse>>> GetPlateStatesByCountryIdsBatchAsync(List<int> countryIds, CancellationToken cancellationToken = default);
        Task<List<PlateStateResponse>> GetFilteredPlateStatesAsync(bool? isActive = null,string? search = null, int? countryId = null,CancellationToken cancellationToken = default);
        Task<List<PlateStateResponse>> SearchPlateStatesAsync(string searchTerm, bool? isActive = null, CancellationToken cancellationToken = default);
    }
}

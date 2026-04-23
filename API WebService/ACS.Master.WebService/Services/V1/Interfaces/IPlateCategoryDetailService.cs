using ACS.Models.Response.V1.MasterService.PlateCategoryDetail;

namespace ACS.Master.WebService.Services.V1.Interfaces
{
    public interface IPlateCategoryDetailService
    {
        Task<List<PlateCategoryDetailRespones>> GetAllPlateCategoryDetailsAsync(bool? isActive = null, CancellationToken cancellationToken = default);
        Task<PlateCategoryDetailRespones?> GetPlateCategoryDetailByKeysAsync(int plateStateId, int countryId, int plateCategoryId, CancellationToken cancellationToken = default);
        Task<List<PlateCategoryDetailRespones>> GetDetailsByPlateStateIdAsync(int plateStateId, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<List<PlateCategoryDetailRespones>> GetDetailsByCountryIdAsync(int countryId, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<List<PlateCategoryDetailRespones>> GetDetailsByCategoryIdAsync(int plateCategoryId, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<Dictionary<(int, int, int), PlateCategoryDetailRespones>> GetDetailsByKeysBatchAsync(List<(int PlateStateId, int CountryId, int PlateCategoryId)> keys, CancellationToken cancellationToken = default);
        Task<Dictionary<int, List<PlateCategoryDetailRespones>>> GetDetailsByCountryIdsBatchAsync(List<int> countryIds, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<Dictionary<int, List<PlateCategoryDetailRespones>>> GetDetailsByPlateStateIdsBatchAsync(List<int> plateStateIds, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<Dictionary<int, List<PlateCategoryDetailRespones>>> GetDetailsByCategoryIdsBatchAsync(List<int> plateCategoryIds, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<List<PlateCategoryDetailRespones>> GetFilteredPlateCategoryDetailsAsync(bool? isActive = null, int? plateStateId = null, int? countryId = null, int? plateCategoryId = null, CancellationToken cancellationToken = default);
        Task<List<PlateCategoryDetailRespones>> GetActiveDetailsByStateAndCategoryAsync(int plateStateId, int plateCategoryId, CancellationToken cancellationToken = default);
    }
}

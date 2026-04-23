using ACS.BusinessEntities.MasterService.V1.PlateCategory;
using ACS.Models.Response.V1.MasterService.PlateCategory;

namespace ACS.Master.WebService.Services.V1.Interfaces
{
    public interface IPlateCategoryService
    {
        Task<List<PlateCategoryResponse>> GetAllPlateCategoriesAsync(bool? isActive = null, CancellationToken cancellationToken = default);
        Task<PlateCategoryResponse?> GetPlateCategoryByIdAsync(int plateCategoryId, CancellationToken cancellationToken = default);
        Task<Dictionary<int, PlateCategoryResponse>> GetPlateCategoriesByIdsBatchAsync(List<int> plateCategoryIds, CancellationToken cancellationToken = default);
        Task<List<PlateCategoryResponse>> GetPlateCategoriesByCodeAsync(string categoryCode, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<List<PlateCategoryResponse>> SearchPlateCategoriesAsync(string searchTerm, bool? isActive = null, CancellationToken cancellationToken = default);
    }
}

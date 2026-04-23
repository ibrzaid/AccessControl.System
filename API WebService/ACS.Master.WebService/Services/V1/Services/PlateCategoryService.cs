using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.MasterService.V1;
using ACS.Master.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.PlateCategory;

namespace ACS.Master.WebService.Services.V1.Services
{
    public class PlateCategoryService(ILicenseManager licenseManager) : Service.Service(licenseManager), IPlateCategoryService
    {

        private IPlateCategoryDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.MasterService.V1.PlateCategoryDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in PlateCategoryService.")
                };
            }
        }

        public async Task<List<PlateCategoryResponse>> GetAllPlateCategoriesAsync(bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetAllPlateCategoriesAsync(isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];
            return [.. (from  category in result
                        select new PlateCategoryResponse
                        {
                            PlateCategoryId = category.PlateCategoryId,
                            CategoryCode = category.CategoryCode,
                            CategoryNames = category.CategoryNames,
                            CategoryDescriptions = category.CategoryDescriptions,
                            IsActive = category.IsActive,
                            CreatedDate = category.CreatedDate,
                            UpdatedDate = category.UpdatedDate,
                        })];
        }

        public async Task<Dictionary<int, PlateCategoryResponse>> GetPlateCategoriesByIdsBatchAsync(List<int> plateCategoryIds, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetPlateCategoriesByIdsBatchAsync(plateCategoryIds, cancellationToken);
            if (result is null || result.Count == 0) return [];
            return result.ToDictionary(
                kvp => kvp.Key,
                kvp =>new PlateCategoryResponse
                {
                    PlateCategoryId = kvp.Value.PlateCategoryId,
                    CategoryCode = kvp.Value.CategoryCode,
                    CategoryNames = kvp.Value.CategoryNames,
                    CategoryDescriptions = kvp.Value.CategoryDescriptions,
                    IsActive = kvp.Value.IsActive,
                    CreatedDate = kvp.Value.CreatedDate,
                    UpdatedDate = kvp.Value.UpdatedDate,
                });
        }

        public async Task<PlateCategoryResponse?> GetPlateCategoryByIdAsync(int plateCategoryId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetPlateCategoryByIdAsync(plateCategoryId, cancellationToken);
            if (result is null) return null;
            return new PlateCategoryResponse
            {
                PlateCategoryId = result.PlateCategoryId,
                CategoryCode = result.CategoryCode,
                CategoryNames = result.CategoryNames,
                CategoryDescriptions = result.CategoryDescriptions,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                UpdatedDate = result.UpdatedDate,
            };
        }

        public async Task<List<PlateCategoryResponse>> GetPlateCategoriesByCodeAsync(string categoryCode, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetPlateCategoriesByCodeAsync(categoryCode, isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(category => new PlateCategoryResponse
            {
                PlateCategoryId = category.PlateCategoryId,
                CategoryCode = category.CategoryCode,
                CategoryNames = category.CategoryNames,
                CategoryDescriptions = category.CategoryDescriptions,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate,
                PlateCategoryDetails = []
            })];
        }

        public async Task<List<PlateCategoryResponse>> SearchPlateCategoriesAsync(string searchTerm, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].SearchPlateCategoriesAsync(searchTerm, isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(category => new PlateCategoryResponse
            {
                PlateCategoryId = category.PlateCategoryId,
                CategoryCode = category.CategoryCode,
                CategoryNames = category.CategoryNames,
                CategoryDescriptions = category.CategoryDescriptions,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate,
                PlateCategoryDetails = []
            })];
        }

    }
}

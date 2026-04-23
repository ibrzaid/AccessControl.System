using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.MasterService.V1;
using ACS.Master.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.County;
using ACS.Models.Response.V1.MasterService.PlateState;
using ACS.Models.Response.V1.MasterService.PlateCategory;
using ACS.Models.Response.V1.MasterService.PlateCategoryDetail;

namespace ACS.Master.WebService.Services.V1.Services
{
    public class PlateCategoryDetailService(ILicenseManager licenseManager) : Service.Service(licenseManager), IPlateCategoryDetailService
    {
        private IPlateCategoryDetailDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.MasterService.V1.PlateCategoryDetailDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in PlateCategoryDetailService.")
                };
            }
        }

        public async Task<List<PlateCategoryDetailRespones>> GetAllPlateCategoryDetailsAsync(bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetAllPlateCategoryDetailsAsync(isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(detail => MapToResponse(detail))];
        }

        public async Task<PlateCategoryDetailRespones?> GetPlateCategoryDetailByKeysAsync(int plateStateId, int countryId, int plateCategoryId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetPlateCategoryDetailByKeysAsync(plateStateId, countryId, plateCategoryId, cancellationToken);
            if (result is null) return null;

            return MapToResponse(result);
        }

        public async Task<List<PlateCategoryDetailRespones>> GetDetailsByPlateStateIdAsync(int plateStateId, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDetailsByPlateStateIdAsync(plateStateId, isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(detail => MapToResponse(detail))];
        }

        public async Task<List<PlateCategoryDetailRespones>> GetDetailsByCountryIdAsync(int countryId, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDetailsByCountryIdAsync(countryId, isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(detail => MapToResponse(detail))];
        }

        public async Task<List<PlateCategoryDetailRespones>> GetDetailsByCategoryIdAsync(int plateCategoryId, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDetailsByCategoryIdAsync(plateCategoryId, isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(detail => MapToResponse(detail))];
        }

        public async Task<Dictionary<(int, int, int), PlateCategoryDetailRespones>> GetDetailsByKeysBatchAsync(List<(int PlateStateId, int CountryId, int PlateCategoryId)> keys, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDetailsByKeysBatchAsync(keys, cancellationToken);

            if (result is null || result.Count == 0) return [];

            return result.ToDictionary(
                kvp => kvp.Key,
                kvp => MapToResponse(kvp.Value)
            );
        }

        public async Task<Dictionary<int, List<PlateCategoryDetailRespones>>> GetDetailsByCountryIdsBatchAsync( List<int> countryIds, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDetailsByCountryIdsBatchAsync(countryIds, isActive, cancellationToken);

            if (result is null || result.Count == 0) return new Dictionary<int, List<PlateCategoryDetailRespones>>();

            return result.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(MapToResponse).ToList()
            );
        }

        public async Task<Dictionary<int, List<PlateCategoryDetailRespones>>> GetDetailsByPlateStateIdsBatchAsync(List<int> plateStateIds,bool? isActive = null,CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDetailsByPlateStateIdsBatchAsync(plateStateIds, isActive, cancellationToken);

            if (result is null || result.Count == 0) return new Dictionary<int, List<PlateCategoryDetailRespones>>();

            return result.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(MapToResponse).ToList()
            );
        }

        public async Task<Dictionary<int, List<PlateCategoryDetailRespones>>> GetDetailsByCategoryIdsBatchAsync( List<int> plateCategoryIds,bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDetailsByCategoryIdsBatchAsync(plateCategoryIds, isActive, cancellationToken);

            if (result is null || result.Count == 0) return [];

            return result.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(MapToResponse).ToList()
            );
        }

        public async Task<List<PlateCategoryDetailRespones>> GetFilteredPlateCategoryDetailsAsync( bool? isActive = null, int? plateStateId = null,  int? countryId = null,  int? plateCategoryId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetFilteredPlateCategoryDetailsAsync(
               isActive, plateStateId, countryId, plateCategoryId, cancellationToken);
            if (result is null || result.Count == 0) return [];
            return [.. result.Select(MapToResponse)];
        }

        public async Task<List<PlateCategoryDetailRespones>> GetActiveDetailsByStateAndCategoryAsync( int plateStateId,  int plateCategoryId, CancellationToken cancellationToken = default)
        {
            return await GetFilteredPlateCategoryDetailsAsync(
                isActive: true,
                plateStateId: plateStateId,
                plateCategoryId: plateCategoryId,
                cancellationToken: cancellationToken);
        }


        private static PlateCategoryDetailRespones MapToResponse(ACS.BusinessEntities.MasterService.V1.PlateCategoryDetail.PlateCategoryDetailEntity entity)
        {
            return new PlateCategoryDetailRespones
            {
                PlateStateId = entity.PlateStateId,
                CountryId = entity.CountryId,
                PlateCategoryId = entity.PlateCategoryId,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                PlateState = new PlateStateResponse
                {
                    PlateStateId = entity.PlateState.PlateStateId,
                    PlateStateName = entity.PlateState.PlateStateName,
                    CountryId = entity.PlateState.CountryId,
                    IsActive = entity.PlateState.IsActive,
                    CreatedDate = entity.PlateState.CreatedDate,
                    UpdatedDate = entity.PlateState.UpdatedDate
                },
                Country = new CountryResponse
                {
                    CountryId = entity.Country.CountryId,
                    CountryCode = entity.Country.CountryCode,
                    CountryNames = entity.Country.CountryNames,
                    IsActive = entity.Country.IsActive,
                    CreatedDate = entity.Country.CreatedDate,
                    UpdatedDate = entity.Country.UpdatedDate,
                    Alphabets=entity.Country.Alphabets,
                    Digits=entity.Country.Digits,
                    PatternDescription=entity.Country.PatternDescription,
                    PatternRegex=entity.Country.PatternRegex,
                    PlateConfigUpdatedAt = entity.Country.PlateConfigUpdatedAt,
                    PlateConfigCreatedAt = entity.Country.PlateConfigCreatedAt,
                    
                },
                PlateCategory = new PlateCategoryResponse
                {
                    PlateCategoryId = entity.PlateCategory.PlateCategoryId,
                    CategoryCode = entity.PlateCategory.CategoryCode,
                    CategoryNames = entity.PlateCategory.CategoryNames,
                    CategoryDescriptions = entity.PlateCategory.CategoryDescriptions,
                    IsActive = entity.PlateCategory.IsActive,
                    CreatedDate = entity.PlateCategory.CreatedDate,
                    UpdatedDate = entity.PlateCategory.UpdatedDate
                }
            };
        }
    }
}
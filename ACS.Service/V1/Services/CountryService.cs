using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.MasterService.V1;
using ACS.Models.Response.V1.MasterService.County;

namespace ACS.Service.V1.Services
{
    public class CountryService(ILicenseManager licenseManager, IPlateStateService plateStateService) : Service(licenseManager), ICountryService
    {
        private ICountyDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.MasterService.V1.CountyDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in AuthenticationService.")
                };
            }
        }

        public async Task<List<CountryResponse>> GetAllCountriesAsync(bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetAllCountriesAsync(isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];
            return [.. (from  country in result
                                     select new CountryResponse
                                     {
                                         CountryCode = country.CountryCode,
                                         CountryId = country.CountryId,
                                         CountryNames = country.CountryNames,
                                         CreatedDate = country.CreatedDate,
                                         IsActive = country.IsActive,
                                         UpdatedDate = country.UpdatedDate,
                                         Alphabets = country.Alphabets,
                                         Digits = country.Digits,
                                         PatternDescription = country.PatternDescription,
                                         PatternRegex = country.PatternRegex,
                                         PlateConfigCreatedAt = country.PlateConfigCreatedAt,
                                         PlateConfigUpdatedAt = country.PlateConfigUpdatedAt,
                                         PlateCategoryDetails=[],
                                         PlateStates=[]
                                     })];
        }

        public async Task<Dictionary<int, CountryResponse>> GetCountriesByIdsBatchAsync(List<int> countryIds, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetCountriesByIdsBatchAsync(countryIds, cancellationToken);
            if (result is null || result.Count == 0) return [];

            // Get plate states for all countries
            var plateStatesByCountry = await plateStateService.GetPlateStatesByCountryIdsBatchAsync(countryIds, cancellationToken);

            return result.ToDictionary(
                kvp => kvp.Key,
                kvp => new CountryResponse
                {
                    CountryId = kvp.Value.CountryId,
                    CountryCode = kvp.Value.CountryCode,
                    CountryNames = kvp.Value.CountryNames,
                    IsActive = kvp.Value.IsActive,
                    CreatedDate = kvp.Value.CreatedDate,
                    UpdatedDate = kvp.Value.UpdatedDate,
                    PlateStates = plateStatesByCountry.TryGetValue(kvp.Key, out var states) ? states : [],
                    PlateConfigUpdatedAt = kvp.Value.PlateConfigUpdatedAt,
                    PlateConfigCreatedAt = kvp.Value.PlateConfigCreatedAt,
                    PatternRegex = kvp.Value.PatternRegex,
                    PatternDescription = kvp.Value.PatternDescription,
                    Digits = kvp.Value.Digits,
                    Alphabets=kvp.Value.Alphabets,
                    
                    PlateCategoryDetails = [] // You can add this too,
                    
                });
        }

        public async Task<CountryResponse?> GetCountryByIdAsync(int countryId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetCountryByIdAsync(countryId, cancellationToken);
            if (result is null) return null;
            return new CountryResponse
            {
                CountryId = result.CountryId,
                CountryCode = result.CountryCode,
                CountryNames = result.CountryNames,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                UpdatedDate = result.UpdatedDate,
                PlateStates = [],
                PlateCategoryDetails = [],
                Alphabets= result.Alphabets,
                Digits= result.Digits,
                PatternDescription= result.PatternDescription,
                PatternRegex= result.PatternRegex,
                PlateConfigCreatedAt= result.PlateConfigCreatedAt,
                 PlateConfigUpdatedAt= result.PlateConfigUpdatedAt,

            };
        }


        public async Task<List<CountryResponse>> GetCountriesByCodeAsync(string countryCode, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetCountriesByCodeAsync(countryCode, isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(country => new CountryResponse
            {
                CountryId = country.CountryId,
                CountryCode = country.CountryCode,
                CountryNames = country.CountryNames,
                IsActive = country.IsActive,
                CreatedDate = country.CreatedDate,
                UpdatedDate = country.UpdatedDate,
                PlateStates = [],
                PlateCategoryDetails = [],
                Alphabets= country.Alphabets,
                Digits= country.Digits,
                PatternDescription= country.PatternDescription,
                PatternRegex= country.PatternRegex,
                PlateConfigUpdatedAt = country.PlateConfigUpdatedAt,
                PlateConfigCreatedAt  = country.PlateConfigUpdatedAt,
            })];
        }

        public async Task<List<CountryResponse>> SearchCountriesAsync(string searchTerm, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].SearchCountriesAsync(searchTerm, isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(country => new CountryResponse
            {
                CountryId = country.CountryId,
                CountryCode = country.CountryCode,
                CountryNames = country.CountryNames,
                IsActive = country.IsActive,
                CreatedDate = country.CreatedDate,
                UpdatedDate = country.UpdatedDate,
                PlateStates = [],
                PlateCategoryDetails = [],
                PlateConfigCreatedAt= country.PlateConfigUpdatedAt,
                PlateConfigUpdatedAt= country.PlateConfigUpdatedAt,
                PatternRegex = country.PatternRegex,
                PatternDescription = country.PatternDescription,
                Digits = country.Digits,
                Alphabets = country.Alphabets
            })];
        }

    }
}

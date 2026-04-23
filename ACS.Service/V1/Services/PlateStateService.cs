using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.MasterService.V1;
using ACS.Models.Response.V1.MasterService.County;
using ACS.Models.Response.V1.MasterService.PlateState;

namespace ACS.Service.V1.Services
{
    public class PlateStateService(ILicenseManager licenseManager) : Service(licenseManager), IPlateStateService
    {
        private IPlateStateDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.MasterService.V1.PlateStateDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in PlateStateService.")
                };
            }
        }

        public  async Task<List<PlateStateResponse>> GetAllPlateStatesAsync(bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetAllPlateStatesAsync(isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];
            return [.. (from  state in result
                                     select new PlateStateResponse
                                     {
                                         Country = state.Country != null ? new CountryResponse
                                         {
                                            CountryCode = state.Country.CountryCode,
                                            CountryId = state.Country.CountryId,
                                            CountryNames = state.Country.CountryNames,
                                            CreatedDate = state.Country.CreatedDate,
                                            UpdatedDate = state.Country.UpdatedDate,
                                            IsActive = state.Country.IsActive,
                                            PlateConfigCreatedAt = state.Country.PlateConfigCreatedAt,
                                            Alphabets = state.Country.Alphabets,
                                            Digits = state.Country.Digits,
                                            PatternDescription = state.Country.PatternDescription,
                                            PatternRegex= state.Country.PatternRegex,
                                             PlateConfigUpdatedAt= state.Country.PlateConfigUpdatedAt
                                            
                                         } : null,
                                         CountryId = state.CountryId,
                                         CreatedDate =  state.CreatedDate,
                                         IsActive = state.IsActive,
                                         PlateStateId = state.PlateStateId,
                                         PlateStateName = state.PlateStateName,
                                         UpdatedDate = state.UpdatedDate                                         
                                     })];
        }

        public async Task<PlateStateResponse?> GetPlateStateByIdAsync(int plateStateId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetPlateStateByIdAsync(plateStateId, cancellationToken);
            if (result is null) return null;
            return new PlateStateResponse
            {
                Country = result.Country != null ? new CountryResponse
                {
                    CountryCode = result.Country.CountryCode,
                    CountryId = result.Country.CountryId,
                    CountryNames = result.Country.CountryNames,
                    CreatedDate = result.Country.CreatedDate,
                    UpdatedDate = result.Country.UpdatedDate,
                    IsActive = result.Country.IsActive,
                    PlateConfigCreatedAt = result.Country.PlateConfigCreatedAt,
                    Alphabets = result.Country.Alphabets,
                    Digits = result.Country.Digits,
                    PatternDescription = result.Country.PatternDescription,
                    PatternRegex = result.Country.PatternRegex,
                    PlateConfigUpdatedAt = result.Country.PlateConfigUpdatedAt
                } : null,
                CountryId = result.CountryId,
                CreatedDate = result.CreatedDate,
                IsActive = result.IsActive,
                PlateStateId = result.PlateStateId,
                PlateStateName = result.PlateStateName,
                UpdatedDate = result.UpdatedDate
            };
        }

        public async Task<List<PlateStateResponse>> GetPlateStatesByCountryIdAsync(int countryId, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetPlateStatesByCountryIdAsync(countryId, isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];
            return [.. (from state in result
                                     select new PlateStateResponse
                                     {
                                         Country = state.Country != null ? new CountryResponse
                                         {
                                            CountryCode = state.Country.CountryCode,
                                            CountryId = state.Country.CountryId,
                                            CountryNames = state.Country.CountryNames,
                                            CreatedDate = state.Country.CreatedDate,
                                            UpdatedDate = state.Country.UpdatedDate,
                                            IsActive = state.Country.IsActive,
                                             PlateConfigCreatedAt = state.Country.PlateConfigCreatedAt,
                                            Alphabets = state.Country.Alphabets,
                                            Digits = state.Country.Digits,
                                            PatternDescription = state.Country.PatternDescription,
                                            PatternRegex= state.Country.PatternRegex,
                                             PlateConfigUpdatedAt= state.Country.PlateConfigUpdatedAt

                                         } : null,
                                         CountryId = state.CountryId,
                                         CreatedDate =  state.CreatedDate,
                                         IsActive = state.IsActive,
                                         PlateStateId = state.PlateStateId,
                                         PlateStateName = state.PlateStateName,
                                         UpdatedDate = state.UpdatedDate                                         
                                     })];
        }

        public async Task<Dictionary<int, PlateStateResponse>> GetPlateStatesByIdsBatchAsync(List<int> plateStateIds, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetPlateStatesByIdsBatchAsync(plateStateIds, cancellationToken);
            if (result is null || result.Count == 0) return [];
            return result.ToDictionary(
                kvp => kvp.Key,
                kvp => new PlateStateResponse
                {
                    PlateStateId = kvp.Value.PlateStateId,
                    PlateStateName = kvp.Value.PlateStateName,
                    IsActive = kvp.Value.IsActive,
                    UpdatedDate = kvp.Value.UpdatedDate,
                    CountryId = kvp.Value.CountryId,
                    CreatedDate = kvp.Value.CreatedDate,
                    Country = kvp.Value.Country != null
                        ? new CountryResponse
                        {
                            CountryCode = kvp.Value.Country.CountryCode,
                            CountryId = kvp.Value.Country.CountryId,
                            CountryNames = kvp.Value.Country.CountryNames,
                            CreatedDate = kvp.Value.Country.CreatedDate,
                            IsActive = kvp.Value.Country.IsActive,
                            UpdatedDate = kvp.Value.Country.UpdatedDate,
                             PlateConfigCreatedAt = kvp.Value.Country.PlateConfigCreatedAt,
                            Alphabets = kvp.Value.Country.Alphabets,
                            Digits = kvp.Value.Country.Digits,
                            PatternDescription = kvp.Value.Country.PatternDescription,
                            PatternRegex = kvp.Value.Country.PatternRegex,
                            PlateConfigUpdatedAt = kvp.Value.Country.PlateConfigUpdatedAt
                        }
                        : null
                });
        }

        public async Task<Dictionary<int, List<PlateStateResponse>>> GetPlateStatesByCountryIdsBatchAsync(List<int> countryIds, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetPlateStatesByCountryIdsBatchAsync(countryIds, cancellationToken);

            if (result is null || result.Count == 0) return [];

            return result.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(state => new PlateStateResponse
                {
                    PlateStateId = state.PlateStateId,
                    PlateStateName = state.PlateStateName,
                    CountryId = state.CountryId,
                    IsActive = state.IsActive,
                    CreatedDate = state.CreatedDate,
                    UpdatedDate = state.UpdatedDate,
                    Country = null, // Don't load circular reference here
                    PlateCategoryDetails = []
                }).ToList()
            );
        }


        public async Task<List<PlateStateResponse>> GetFilteredPlateStatesAsync(bool? isActive = null,string? search = null, int? countryId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetFilteredPlateStatesAsync(isActive, search, countryId, cancellationToken);
             
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(state => new PlateStateResponse
            {
                PlateStateId = state.PlateStateId,
                PlateStateName = state.PlateStateName,
                CountryId = state.CountryId,
                IsActive = state.IsActive,
                CreatedDate = state.CreatedDate,
                UpdatedDate = state.UpdatedDate,
                Country = state.Country != null ? new CountryResponse
                {
                    CountryId = state.Country.CountryId,
                    CountryCode = state.Country.CountryCode,
                    CountryNames = state.Country.CountryNames,
                    IsActive = state.Country.IsActive,
                    CreatedDate = state.Country.CreatedDate,
                    UpdatedDate = state.Country.UpdatedDate,
                     PlateConfigCreatedAt = state.Country.PlateConfigCreatedAt,
                                            Alphabets = state.Country.Alphabets,
                                            Digits = state.Country.Digits,
                                            PatternDescription = state.Country.PatternDescription,
                                            PatternRegex= state.Country.PatternRegex,
                                             PlateConfigUpdatedAt= state.Country.PlateConfigUpdatedAt
                } : null,
                PlateCategoryDetails = []
            })];
        }

        public async Task<List<PlateStateResponse>> SearchPlateStatesAsync(string searchTerm, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].SearchPlateStatesAsync(searchTerm, isActive, cancellationToken);
            if (result is null || result.Count == 0) return [];

            return [.. result.Select(state => new PlateStateResponse
            {
                PlateStateId = state.PlateStateId,
                PlateStateName = state.PlateStateName,
                CountryId = state.CountryId,
                IsActive = state.IsActive,
                CreatedDate = state.CreatedDate,
                UpdatedDate = state.UpdatedDate,
                Country = null, 
                PlateCategoryDetails = []
            })];
        }

    }
}

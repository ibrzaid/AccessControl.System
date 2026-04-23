using ACS.Models.Response.V1.MasterService.County;

namespace ACS.Service.V1.Interfaces
{
    public interface ICountryService
    {
        Task<List<CountryResponse>> GetAllCountriesAsync(bool? isActive = null, CancellationToken cancellationToken = default);
        Task<CountryResponse?> GetCountryByIdAsync(int countryId, CancellationToken cancellationToken = default);
        Task<Dictionary<int, CountryResponse>> GetCountriesByIdsBatchAsync(List<int> countryIds, CancellationToken cancellationToken = default);
        Task<List<CountryResponse>> GetCountriesByCodeAsync(string countryCode, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<List<CountryResponse>> SearchCountriesAsync(string searchTerm, bool? isActive = null, CancellationToken cancellationToken = default);
    }
}

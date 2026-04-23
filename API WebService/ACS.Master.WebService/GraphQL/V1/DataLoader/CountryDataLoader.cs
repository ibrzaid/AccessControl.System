using GraphQL.DataLoader;
using ACS.Service.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.County;

namespace ACS.Master.WebService.GraphQL.V1.DataLoader
{
    public class CountryDataLoader(
        ICountryService countryService,
        IEqualityComparer<int>? keyComparer = null,
        int maxBatchSize = 100) : BatchDataLoader<int, CountryResponse>(
            fetchDelegate: async (keys, cancellationToken) =>
                {
                    var result = await countryService.GetCountriesByIdsBatchAsync(
                        [.. keys],
                        cancellationToken);

                    return result;
                },
            keyComparer: keyComparer,
            defaultValue: null!,
            maxBatchSize: maxBatchSize)
    {

        public async Task<CountryResponse?> LoadSingleAsync(int countryId, CancellationToken cancellationToken = default)
        {
            return await LoadAsync(countryId).GetResultAsync(cancellationToken);
        }
    }
}
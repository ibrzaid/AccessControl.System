using GraphQL.DataLoader;
using ACS.Service.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.PlateState;

namespace ACS.Master.WebService.GraphQL.V1.DataLoader
{
    public class CountryPlateStatesDataLoader(
        IPlateStateService plateStateService,
        IEqualityComparer<int>? keyComparer = null,
        int maxBatchSize = 100) : BatchDataLoader<int, List<PlateStateResponse>>(
            fetchDelegate: async (keys, cancellationToken) =>
                {
                    var result = await plateStateService.GetPlateStatesByCountryIdsBatchAsync(
                        [.. keys],
                        cancellationToken);

                    return result;
                },
            keyComparer: keyComparer,
            defaultValue: [],
            maxBatchSize: maxBatchSize)
    {

        public async Task<List<PlateStateResponse>?> LoadSingleAsync(int countryId, CancellationToken cancellationToken = default)
        {
            return await LoadAsync(countryId).GetResultAsync(cancellationToken);
        }
    }
}
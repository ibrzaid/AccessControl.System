using GraphQL.DataLoader;
using ACS.Service.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.PlateState;

namespace ACS.Master.WebService.GraphQL.V1.DataLoader
{
    public class PlateStateDataLoader(
        IPlateStateService plateStateService,
        IEqualityComparer<int>? keyComparer = null,
        int maxBatchSize = 100) : BatchDataLoader<int, PlateStateResponse>(
            fetchDelegate: async (keys, cancellationToken) =>
                {
                    var result = await plateStateService.GetPlateStatesByIdsBatchAsync(
                        [.. keys],
                        cancellationToken);

                    return result;
                },
            keyComparer: keyComparer,
            defaultValue: null!,
            maxBatchSize: maxBatchSize)
    {

        public async Task<PlateStateResponse?> LoadSingleAsync(int plateStateId, CancellationToken cancellationToken = default)
        {
            return await LoadAsync(plateStateId).GetResultAsync(cancellationToken);
        }
    }
}
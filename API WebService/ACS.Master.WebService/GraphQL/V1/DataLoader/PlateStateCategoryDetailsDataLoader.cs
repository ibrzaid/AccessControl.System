using GraphQL.DataLoader;
using ACS.Master.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.PlateCategoryDetail;

namespace ACS.Master.WebService.GraphQL.V1.DataLoader
{
    public class PlateStateCategoryDetailsDataLoader(
        IPlateCategoryDetailService plateCategoryDetailService,
        IEqualityComparer<int>? keyComparer = null,
        int maxBatchSize = 100) : BatchDataLoader<int, List<PlateCategoryDetailRespones>>(
            fetchDelegate: async (keys, cancellationToken) =>
                {
                    var result = await plateCategoryDetailService.GetDetailsByPlateStateIdsBatchAsync(
                        [.. keys],
                        null,
                        cancellationToken);

                    return result;
                },
            keyComparer: keyComparer,
            defaultValue: [],
            maxBatchSize: maxBatchSize)
    {

        public async Task<List<PlateCategoryDetailRespones>?> LoadSingleAsync(int plateStateId, CancellationToken cancellationToken = default)
        {
            return await LoadAsync(plateStateId).GetResultAsync(cancellationToken);
        }
    }
}
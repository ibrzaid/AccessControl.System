using GraphQL.DataLoader;
using ACS.Master.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.PlateCategory;

namespace ACS.Master.WebService.GraphQL.V1.DataLoader
{
   public class PlateCategoryDataLoader(
       IPlateCategoryService plateCategoryService,
       IEqualityComparer<int>? keyComparer = null,
       int maxBatchSize = 100) : BatchDataLoader<int, PlateCategoryResponse>(
           fetchDelegate: async (keys, cancellationToken) =>
                {
                    var result = await plateCategoryService.GetPlateCategoriesByIdsBatchAsync(
                        [.. keys],
                        cancellationToken);

                    return result;
                },
           keyComparer: keyComparer,
           defaultValue: null!,
           maxBatchSize: maxBatchSize)
    {

        public async Task<PlateCategoryResponse?> LoadSingleAsync(int plateCategoryId, CancellationToken cancellationToken = default)
        {
            return await LoadAsync(plateCategoryId).GetResultAsync(cancellationToken);
        }
    }
}
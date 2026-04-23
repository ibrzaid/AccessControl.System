using ACS.Master.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.PlateCategoryDetail;
using GraphQL.DataLoader;

namespace ACS.Master.WebService.GraphQL.V1.DataLoader
{

    public class TupleEqualityComparer : IEqualityComparer<(int, int, int)>
    {
        public bool Equals((int, int, int) x, (int, int, int) y)
        {
            return x.Item1 == y.Item1 &&
                   x.Item2 == y.Item2 &&
                   x.Item3 == y.Item3;
        }

        public int GetHashCode((int, int, int) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2, obj.Item3);
        }
    }

    public class PlateCategoryDetailDataLoader(
        IPlateCategoryDetailService plateCategoryDetailService,
        IEqualityComparer<(int, int, int)>? keyComparer = null,
        int maxBatchSize = 100) : BatchDataLoader<(int, int, int), PlateCategoryDetailRespones>(
            fetchDelegate: async (keys, cancellationToken) =>
                {
                    var keysList = keys.ToList();
                    var result = await plateCategoryDetailService.GetDetailsByKeysBatchAsync(
                        keysList,
                        cancellationToken);

                    return result;
                },
            keyComparer: keyComparer ?? new TupleEqualityComparer(),
            defaultValue: null!,
            maxBatchSize: maxBatchSize)
    {

        public async Task<PlateCategoryDetailRespones?> LoadSingleAsync(
            int plateStateId,
            int countryId,
            int plateCategoryId,
            CancellationToken cancellationToken = default)
        {
            var key = (plateStateId, countryId, plateCategoryId);
            return await LoadAsync(key).GetResultAsync(cancellationToken);
        }
    }
}
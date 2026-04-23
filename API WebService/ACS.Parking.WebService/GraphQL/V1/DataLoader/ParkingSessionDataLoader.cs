using GraphQL.DataLoader;
using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Parking.WebService.Services.V1.Interfaces;

namespace ACS.Parking.WebService.GraphQL.V1.DataLoader
{
    public class ParkingSessionDataLoader(
        ISessionService sessionService,
        IEqualityComparer<int>? keyComparer = null,
        int maxBatchSize = 100) : BatchDataLoader<int, ParkingSessionDataResponse?>(
            fetchDelegate: async (keys, cancellationToken) =>
            {
                return null;
            },
            keyComparer: keyComparer,
            defaultValue: null!,
            maxBatchSize: maxBatchSize
            )
    {
        public async Task<ParkingSessionDataResponse?> LoadSingleAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return await LoadAsync(sessionId).GetResultAsync(cancellationToken);
        }
    }
}

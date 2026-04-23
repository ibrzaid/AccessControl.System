using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Parking.WebService.Services.V1.Interfaces;
using GraphQL.DataLoader;

namespace ACS.Parking.WebService.GraphQL.V1.DataLoader
{
    public class ZoneSessionsDataLoader(
        ISessionService sessionService,
        IEqualityComparer<int>? keyComparer = null,
        int maxBatchSize = 100) : BatchDataLoader<int, IReadOnlyList<ParkingSessionDataResponse>>(
             fetchDelegate: async (keys, cancellationToken) =>
             {
                 return null;
             },
            keyComparer: keyComparer,
            defaultValue: null!,
            maxBatchSize: maxBatchSize
            )
    {
    }
}

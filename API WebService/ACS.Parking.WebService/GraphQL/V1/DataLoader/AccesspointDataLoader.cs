using ACS.Helper.V1;
using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Parking.WebService.Services.V1.Interfaces;
using GraphQL.DataLoader;

namespace ACS.Parking.WebService.GraphQL.V1.DataLoader
{
    public class AccesspointDataLoader(
       ISessionService sessionService,
       IHttpContextAccessor httpContextAccessor,
       FindClaimHelper findClaimHelper,
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
    }
}

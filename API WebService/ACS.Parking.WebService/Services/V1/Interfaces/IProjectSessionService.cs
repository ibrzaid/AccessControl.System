using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Models.Response.V1.ParkingService.Session;

namespace ACS.Parking.WebService.Services.V1.Interfaces
{
    public interface IProjectSessionService
    {

        Task<SessionsResponse> GetProjectSessionsForBatchAsync(
           int workspace,
           int project,
           int skip,
           int take,
           string? status,
           CancellationToken cancellationToken = default);

        Task AddSessionToCacheAsync(
            int workspace,
            int project,
            ParkingSessionDataResponse session,
            string? status = null,
            CancellationToken cancellationToken = default);


        Task CloseOldSessionsInCacheAsync(
           int workspace,
           int project,
           string plateCode,
           string plateNumber,
           int country,
           int state,
           int category,
           string excludeSessionId,
           CancellationToken cancellationToken);


        Task AddSessionIdToListAsync(
            int workspace,
            int project,
            string? status,
            string sessionId,
            CancellationToken cancellationToken);


        Task IncrementTotalCountAsync(
            int workspace,
            int project,
            string? status,
            CancellationToken cancellationToken);

        public Task<SessionsResponse> GetSessionsPageAsync(
            int workspace,
            int project,
            int skip,
            int take,
            string? status,
            Func<int, int, Task<List<ParkingSessionDataResponse>>> getDatabaseSessionsFunc,
            CancellationToken cancellationToken = default);


    }
}

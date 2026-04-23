using ACS.Models.Response.V1.AuthenticationService.Timezone;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface ITimezoneService
    {
        /// <summary>
        /// All PostgreSQL timezone names grouped by region. Pass search to filter.
        /// </summary>
        /// <param name="search"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetTimezonesResponse> GetTimezonesAsync(
            string? search = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Effective timezone, date format, and language for a user.
        /// User setting → workspace fallback → UTC / YYYY-MM-DD / en-US.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DisplaySettingsResponse> GetDisplaySettingsAsync(
            int userId,
            int workspaceId,
            CancellationToken cancellationToken = default);
    }
}

using ACS.Models.Response.V1.AuthenticationService.License;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface ILicenseService
    {
        /// <summary>
        /// Validate license and seat availability for the workspace.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LicenseCheckResponse> CheckLicenseAsync(
            int workspaceId,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Seat usage summary for the admin dashboard widget.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SeatSummaryResponse> GetSeatSummaryAsync(
            int workspaceId,
            CancellationToken cancellationToken = default);
    }
}

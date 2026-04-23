using ACS.Models.Response.V1.AuthenticationService.UserStatus;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IUserStatusService
    {
        /// <summary>
        /// All user status options (ACTIVE, INACTIVE, SUSPENDED, LOCKED …).
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetStatusesResponse> GetUserStatusesAsync(
            CancellationToken cancellationToken = default);
    }
}

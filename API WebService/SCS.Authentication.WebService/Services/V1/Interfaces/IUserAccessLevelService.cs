using ACS.Models.Response.V1.AuthenticationService.AccessLevel;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IUserAccessLevelService
    {
        /// <summary>
        /// All access levels (VIEWER, OPERATOR, MANAGER, ADMIN, SUPER_ADMIN).
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetAccessLevelsResponse> GetAccessLevelsAsync(
            CancellationToken cancellationToken = default);
    }
}

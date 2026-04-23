using ACS.Models.Response.V1.AuthenticationService.AccessScope;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IUserAccessScopeService
    {
        /// <summary>
        /// All scope types (WORKSPACE, PROJECT, PROJECT_AREA, ZONE, ACCESS_POINT).
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetAccessScopesResponse> GetAccessScopesAsync(
            CancellationToken cancellationToken = default);
    }
}

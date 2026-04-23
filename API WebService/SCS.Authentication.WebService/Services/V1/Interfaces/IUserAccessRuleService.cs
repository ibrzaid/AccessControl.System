using ACS.Models.Response.V1.AuthenticationService.AccessRule;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IUserAccessRuleService
    {
        /// <summary>
        /// Get all active access rules. userId=0 → all users in workspace.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="callerUserId"></param>
        /// <param name="userId"></param>
        /// <param name="includeBreadcrumbs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetAccessRulesResponse> GetAccessRulesAsync(
            int workspaceId,
            int callerUserId,
            int userId = 0,
            bool includeBreadcrumbs = false,
            CancellationToken cancellationToken = default);
    }
}

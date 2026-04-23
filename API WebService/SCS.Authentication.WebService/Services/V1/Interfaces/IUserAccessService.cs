using ACS.Models.Request.V1.AuthenticationService.AccessUser;
using ACS.Models.Response.V1.AuthenticationService.UserAccess;
using ACS.Models.Response.V1.AuthenticationService.UserManagement;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IUserAccessService
    {
        /// <summary>
        /// Paginated access audit history. userId=0 → all users.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetAccessHistoryResponse> GetAccessHistoryAsync(
            int workspaceId,
            int userId = 0,
            int callerUserId = 0,
            int limit = 100,
            int offset = 0,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Grant or upsert user access.
        /// Never downgrades an existing higher-level rule.
        /// Auto-creates breadcrumb rows for parent nodes.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GrantAccessResponse> GrantAccessAsync(
            int callerUserId,
            int workspaceId,
            GrantAccessRequest request,
            string? ipAddress = null,
            string? deviceInfo = null,
            string? agent = null,
            string? requestId = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Soft-revoke a single access rule by ID.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="userAccessId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserOperationResponse> RevokeAccessAsync(
           int callerUserId,
           long userAccessId,
           int workspaceId,
           RevokeAccessRequest request,
           string? ipAddress = null,
           string? deviceInfo = null,
           string? agent = null,
           string? requestId = null,
           CancellationToken cancellationToken = default);
    }
}

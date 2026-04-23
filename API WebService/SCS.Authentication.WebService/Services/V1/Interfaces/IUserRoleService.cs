using ACS.Models.Request.V1.AuthenticationService.UserRole;
using ACS.Models.Response.V1.AuthenticationService.UserRole;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IUserRoleService
    {
        /// <summary>
        ///  All active roles in a workspace with user counts.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetRolesResponse> GetRolesAsync(
            int workspaceId, int roleId = 0,
            CancellationToken cancellationToken = default);


        Task<GetRoleResponse> GetRoleByIdAsync(
            int roleId, int workspaceId,
            CancellationToken cancellationToken = default);

        Task<RoleOperationResponse> CreateRoleAsync(
            int callerUserId, int workspaceId,
            CreateRoleRequest request,
            string? ipAddress, string? userAgent, string? requestId,
            CancellationToken cancellationToken = default);

        Task<RoleOperationResponse> UpdateRoleAsync(
            int callerUserId, int roleId, int workspaceId,
            UpdateRoleRequest request,
            string? ipAddress, string? userAgent, string? requestId,
            CancellationToken cancellationToken = default);

        Task<RoleOperationResponse> DeleteRoleAsync(
            int callerUserId, int roleId, int workspaceId,
            DeleteRoleRequest request,
            string? ipAddress, string? userAgent, string? requestId,
            CancellationToken cancellationToken = default);
    }
}

using ACS.Models.Request.V1.AuthenticationService.ManageUser;
using ACS.Models.Response;
using ACS.Models.Response.V1.AuthenticationService.Avatar;
using ACS.Models.Response.V1.AuthenticationService.UserManagement;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IUserManagementService
    {
        /// <summary>
        /// List users in a workspace. Supports search, status filter, pagination.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="callerUserId"></param>
        /// <param name="userId"></param>
        /// <param name="search"></param>
        /// <param name="statusId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetUsersResponse> GetUsersAsync(
            int workspaceId,
            int callerUserId,
            int userId = 0,
            string? search = null,
            int statusId = 0,
            int limit = 50,
            int offset = 0,            
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Full detail for a single user — profile, roles, access rules.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetUserResponse> GetUserByIdAsync(
            int userId,
            int workspaceId,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Create a new user. Validates license + seat limit first.
        /// Password hashed with bcrypt in the DB.
        /// Timezone/language fall back to workspace defaults if not supplied. 
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
        Task<CreateUserResponse> CreateUserAsync(
            int callerUserId,
            int workspaceId,
            CreateUserRequest request,
            string? ipAddress = null,
            string? deviceInfo = null,
            string? agent = null,
            string? requestId = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Update profile fields. Only supplied (non-null) fields are changed.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="userId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserOperationResponse> UpdateUserAsync(
            int callerUserId,
            int userId,
            int workspaceId,
            UpdateUserRequest request,
            string? ipAddress = null,
            string? deviceInfo = null,
            string? agent = null,
            string? requestId = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Change user status: ACTIVE | INACTIVE | SUSPENDED | LOCKED.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="userId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserOperationResponse> SetUserStatusAsync(
            int callerUserId,
            int userId,
            int workspaceId,
            SetUserStatusRequest request,
            string? ipAddress = null,
            string? deviceInfo = null,
            string? agent = null,
            string? requestId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft-delete user + revoke all access rules + decrement workspace seat counter.
        /// A user cannot delete themselves.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="userId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserOperationResponse> DeleteUserAsync(
           int callerUserId,
           int userId,
           int workspaceId,
           DeleteUserRequest request,
           string? ipAddress = null,
           string? deviceInfo = null,
           string? agent = null,
           string? requestId = null,
           CancellationToken cancellationToken = default);


        /// <summary>
        ///  Admin password reset. No old-password check. Min 8 chars. Unlocks account.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="userId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserOperationResponse> ResetPasswordAsync(
            int callerUserId,
            int userId,
            int workspaceId,
            ResetPasswordRequest request,
            string? ipAddress = null,
            string? deviceInfo = null,
            string? agent = null,
            string? requestId = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Paginated activity / audit log for a single user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="callerUserId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserActivityResponse> GetUserActivityAsync(
            int userId,
            int workspaceId,
            int callerUserId = 0,
            int limit = 50,
            int offset = 0,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Assign a role to a user. Idempotent.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="userId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserOperationResponse> AssignRoleAsync(
            int callerUserId,
            int userId,
            int workspaceId,
            AssignRoleRequest request,
            string? ipAddress = null,
            string? deviceInfo = null,
            string? agent = null,
            string? requestId = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Remove a role from a user.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="userId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="roleId"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserOperationResponse> RemoveRoleAsync(
            int callerUserId,
            int userId,
            int workspaceId,
            int roleId,
            string? ipAddress = null,
            string? deviceInfo = null,
            string? agent = null,
            string? requestId = null,
            CancellationToken cancellationToken = default);



        /// <summary>
        /// Get all users that the caller can manage (by admin scope overlap).
        /// </summary>
        /// <param name="managerUserId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetManageableUsersResponse> GetManageableUsersAsync(
            int managerUserId,
            int workspaceId,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Check if a manager can manage a specific target user.
        /// </summary>
        /// <param name="managerUserId"></param>
        /// <param name="targetUserId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CanManageUserResponse> CanManageUserAsync(
            int managerUserId,
            int targetUserId,
            int workspaceId,
            CancellationToken cancellationToken = default);


       /// <summary>
       /// 
       /// </summary>
       /// <param name="workspaceId"></param>
       /// <param name="callerUserId"></param>
       /// <param name="user"></param>
       /// <param name="avatar"></param>
       /// <param name="ipAddress"></param>
       /// <param name="deviceInfo"></param>
       /// <param name="agent"></param>
       /// <param name="requestId"></param>
       /// <param name="latitude"></param>
       /// <param name="longitude"></param>
       /// <param name="cancellationToken"></param>
       /// <returns></returns>
        Task<UpdateAvatarResponse> UpdateAvatar(
            int workspaceId, 
            int callerUserId,
            int user, 
            IFormFile? avatar, 
            string? ipAddress, 
            string? deviceInfo, 
            string? agent, 
            string? requestId,
            double? latitude,
            double? longitude,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Transfer management of a user from one manager to another. Validates that caller can manage both users and that new manager has capacity in their admin scope. Idempotent.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="actorUserId"></param>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TransferUserManagerResponse> TransferUserManagerAsync(
            int workspaceId,
            int actorUserId,
            TransferUserManagerRequest request,
            string? ipAddress,
            string? deviceInfo,
            string? agent,
            string? requestId,
            CancellationToken cancellationToken = default);

    }
}

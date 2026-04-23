using ACS.Models.Request.V1.AuthenticationService.AccessException;
using ACS.Models.Response.V1.AuthenticationService.UserAccessException;
using ACS.Models.Response.V1.AuthenticationService.UserManagement;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IUserAccessExceptionService
    {
        /// <summary>
        /// Get all access exceptions. userId=0 → all users.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="userId"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetAccessExceptionsResponse> GetAccessExceptionsAsync(
            int workspaceId,
            int userId = 0,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Add an exception (deny or allow override) for a specific sub-resource.
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
        Task<UserOperationResponse> AddAccessExceptionAsync(
            int callerUserId,
            int workspaceId,
            AddExceptionRequest request,
            string? ipAddress = null,
            string? deviceInfo = null,
            string? agent = null,
            string? requestId = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Delete an access exception by ID.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="exceptionId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserOperationResponse> DeleteAccessExceptionAsync(
           int callerUserId,
           long exceptionId,
           int workspaceId,
           string? ipAddress = null,
           string? deviceInfo = null,
           string? agent = null,
           string? requestId = null,
           CancellationToken cancellationToken = default);
    }
}

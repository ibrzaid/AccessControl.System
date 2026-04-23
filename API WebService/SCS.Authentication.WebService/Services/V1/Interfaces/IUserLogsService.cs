using ACS.Models.Response.V1.AuthenticationService.UserLogs;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IUserLogsService
    {
        /// <summary>
        /// Gets the user logs for a specific user in a workspace, with optional filters for target user, action, and entity type, within a specified date range. The results can be paginated using limit and offset parameters.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="userId"></param>
        /// <param name="targetUser"></param>
        /// <param name="action"></param>
        /// <param name="entityType"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="requestId"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="orderBy"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserLogsResponse> GetUserLogsAsync(int workspaceId,
            int userId,
            int? targetUser,
            string? action,
            string? entityType,
            DateTime fromDate,
            DateTime toDate,
            string requestId,
            int limit = 50,
            int page = 0,
            string? orderBy = "DESC",
            CancellationToken cancellationToken = default);
    }
}

using ACS.Models.Response.V1.NotificationsService.Dashboard;
using ACS.Models.Response.V1.NotificationsService.UserNotifications;

namespace ACS.Notifications.WebService.Services.V1.Interfaces
{
    public interface IDashboardService
    {
        void StartNotify();
        Task<NotificationsDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Paginated notifications for the current user (load-more flow).
        /// </summary>
        Task<UserNotificationsListResponse> GetUserNotificationsAsync(
            string workspace, string user, int limit, int offset, bool unreadOnly,
            string requestId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete one of the user's own notifications.
        /// </summary>
        Task<DeleteUserNotificationResponse> DeleteUserNotificationAsync(
            string workspace, string user, long notificationId,
            string requestId, CancellationToken cancellationToken = default);
    }
}

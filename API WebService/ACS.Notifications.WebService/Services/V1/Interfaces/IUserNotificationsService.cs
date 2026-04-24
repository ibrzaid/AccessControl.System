using ACS.Models.Response.V1.NotificationsService.UserNotifications;

namespace ACS.Notifications.WebService.Services.V1.Interfaces
{
    public interface IUserNotificationsService
    {
        Task<UserNotificationsListResponse> GetUserNotificationsAsync(
            string workspace, string user, int limit, int offset, string filterStatus,
            string? search,
            string? priority, string? type, string? since, string? until,
            string? ip, string? userAgent, string? deviceInfo, string requestId,
            decimal reqLatitude, decimal reqLongitude,
            CancellationToken cancellationToken = default);

        Task<DeleteUserNotificationResponse> DeleteUserNotificationAsync(
            string workspace, string user, long notificationId,
            string? ip, string? userAgent, string? deviceInfo, string requestId,
            decimal reqLatitude, decimal reqLongitude,
            CancellationToken cancellationToken = default);

        Task<MarkUserNotificationReadResponse> MarkUserNotificationReadAsync(
            string workspace, string user, long notificationId,
            string? ip, string? userAgent, string? deviceInfo, string requestId,
            decimal reqLatitude, decimal reqLongitude,
            CancellationToken cancellationToken = default);

        Task<MarkAllUserNotificationsReadResponse> MarkAllUserNotificationsReadAsync(
            string workspace, string user,
            string? ip, string? userAgent, string? deviceInfo, string requestId,
            decimal reqLatitude, decimal reqLongitude,
            CancellationToken cancellationToken = default);
    }
}

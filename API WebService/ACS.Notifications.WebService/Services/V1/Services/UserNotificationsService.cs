using ACS.Database.IDataAccess.NotificationsService.V1;
using ACS.License.V1;
using ACS.Models.Response.V1.NotificationsService.UserNotifications;
using ACS.Notifications.WebService.Services.V1.Interfaces;
using ACS.Service.V1.Interfaces;

namespace ACS.Notifications.WebService.Services.V1.Services
{
    public class UserNotificationsService(ILicenseManager licenseManager, ILogger<UserNotificationsService> logger)
        : Service.Service(licenseManager), IUserNotificationsService
    {
        private IUserNotificationsDataAccess this[Connection conn] => conn.Type switch
        {
            Database.IConnection.DatabaseType.PostgresDatabase =>
                Database.DataAccess.PostgresDataAccess.NotificationsService.V1.UserNotificationsDataAccess
                    .GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
            _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserNotificationsService.")
        };

        public async Task<UserNotificationsListResponse> GetUserNotificationsAsync(
            string workspace, string user, int limit, int offset, string filterStatus,
            string? search,
            string? ip, string? userAgent, string? deviceInfo, string requestId,
            decimal reqLatitude, decimal reqLongitude,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!int.TryParse(workspace, out var wid) || !int.TryParse(user, out var uid))
                {
                    return new UserNotificationsListResponse(
                        Success: false, Message: "Invalid workspace or user identifier",
                        ErrorCode: "INVALID_CLAIMS", RequestId: requestId,
                        GeneratedAt: null, Detail: null, Timezone: null,
                        Limit: limit, Offset: offset,
                        TotalCount: 0, UnreadCount: 0, HasMore: false, Notifications: []);
                }

                var normalised = (filterStatus ?? "all").ToLowerInvariant();
                if (normalised != "all" && normalised != "read" && normalised != "unread")
                    normalised = "all";

                // Trim and bound the search to keep query plans well-behaved
                // and align with the SQL function's own 200-char cap.
                var searchTerm = string.IsNullOrWhiteSpace(search) ? null
                    : (search!.Length > 200 ? search[..200] : search.Trim());

                var license = this.LicenseManager.GetLicense();
                var entity = await this[license!.DB!].GetUserNotificationsAsync(
                    wid, uid, limit, offset, normalised, searchTerm,
                    ip, userAgent, deviceInfo, requestId,
                    reqLatitude, reqLongitude, cancellationToken);

                var items = entity.Notifications?.Select(n => new UserNotificationItem(
                    Id:                n.Id,
                    Title:             n.Title,
                    Message:           n.Message,
                    IsRead:            n.IsRead,
                    ActionUrl:         n.ActionUrl,
                    ActionLabel:       n.ActionLabel,
                    RelatedEntityType: n.RelatedEntityType,
                    RelatedEntityId:   n.RelatedEntityId,
                    Priority:          n.Priority,
                    CreatedAt:         n.CreatedAt,
                    ReadAt:            n.ReadAt
                )).ToArray() ?? [];

                return new UserNotificationsListResponse(
                    Success:       entity.Success,
                    Message:       entity.Message,
                    ErrorCode:     entity.Success ? string.Empty : "DB_ERROR",
                    RequestId:     requestId,
                    GeneratedAt:   entity.GeneratedAt,
                    Detail:        entity.Detail,
                    Timezone:      entity.Timezone,
                    Limit:         entity.Limit == 0 ? limit : entity.Limit,
                    Offset:        entity.Offset,
                    TotalCount:    entity.TotalCount,
                    UnreadCount:   entity.UnreadCount,
                    HasMore:       entity.HasMore,
                    Notifications: items);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUserNotificationsAsync failed (req {req})", requestId);
                return new UserNotificationsListResponse(
                    false, ex.Message, "EXCEPTION", requestId,
                    null, ex.GetType().Name, null, limit, offset, 0, 0, false, []);
            }
        }

        // search arg unused here intentionally; controller uses the overload above.

        public async Task<DeleteUserNotificationResponse> DeleteUserNotificationAsync(
            string workspace, string user, long notificationId,
            string? ip, string? userAgent, string? deviceInfo, string requestId,
            decimal reqLatitude, decimal reqLongitude,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!int.TryParse(workspace, out var wid) || !int.TryParse(user, out var uid))
                {
                    return new DeleteUserNotificationResponse(
                        false, "Invalid workspace or user identifier", "INVALID_CLAIMS",
                        requestId, null, null, null);
                }

                var license = this.LicenseManager.GetLicense();
                var entity = await this[license!.DB!].DeleteUserNotificationAsync(
                    wid, uid, notificationId,
                    ip, userAgent, deviceInfo, requestId,
                    reqLatitude, reqLongitude, cancellationToken);

                if (!entity.Success)
                {
                    var errorCode = entity.Detail == "NOT_FOUND" ? "NOT_FOUND"
                                  : (entity.ErrorCode ?? "DB_ERROR");
                    return new DeleteUserNotificationResponse(
                        false, entity.Message ?? "Unable to delete notification",
                        errorCode, requestId, entity.Detail, null, null);
                }

                return new DeleteUserNotificationResponse(
                    Success:     true,
                    Message:     null,
                    ErrorCode:   string.Empty,
                    RequestId:   requestId,
                    Detail:      null,
                    DeletedId:   entity.DeletedId,
                    UnreadCount: entity.UnreadCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteUserNotificationAsync failed (req {req}, id {id})", requestId, notificationId);
                return new DeleteUserNotificationResponse(
                    false, ex.Message, "EXCEPTION", requestId, ex.GetType().Name, null, null);
            }
        }

        public async Task<MarkUserNotificationReadResponse> MarkUserNotificationReadAsync(
            string workspace, string user, long notificationId,
            string? ip, string? userAgent, string? deviceInfo, string requestId,
            decimal reqLatitude, decimal reqLongitude,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!int.TryParse(workspace, out var wid) || !int.TryParse(user, out var uid))
                {
                    return new MarkUserNotificationReadResponse(
                        false, "Invalid workspace or user identifier", "INVALID_CLAIMS",
                        requestId, null, null, null, null);
                }

                var license = this.LicenseManager.GetLicense();
                var entity = await this[license!.DB!].MarkUserNotificationReadAsync(
                    wid, uid, notificationId,
                    ip, userAgent, deviceInfo, requestId,
                    reqLatitude, reqLongitude, cancellationToken);

                if (!entity.Success)
                {
                    var errorCode = entity.Detail == "NOT_FOUND" ? "NOT_FOUND"
                                  : (entity.ErrorCode ?? "DB_ERROR");
                    return new MarkUserNotificationReadResponse(
                        false, entity.Message ?? "Unable to mark notification as read",
                        errorCode, requestId, entity.Detail, null, null, null);
                }

                return new MarkUserNotificationReadResponse(
                    Success:        true,
                    Message:        null,
                    ErrorCode:      string.Empty,
                    RequestId:      requestId,
                    Detail:         null,
                    NotificationId: entity.NotificationId,
                    WasUnread:      entity.WasUnread,
                    UnreadCount:    entity.UnreadCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MarkUserNotificationReadAsync failed (req {req}, id {id})", requestId, notificationId);
                return new MarkUserNotificationReadResponse(
                    false, ex.Message, "EXCEPTION", requestId, ex.GetType().Name, null, null, null);
            }
        }

        public async Task<MarkAllUserNotificationsReadResponse> MarkAllUserNotificationsReadAsync(
            string workspace, string user,
            string? ip, string? userAgent, string? deviceInfo, string requestId,
            decimal reqLatitude, decimal reqLongitude,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!int.TryParse(workspace, out var wid) || !int.TryParse(user, out var uid))
                {
                    return new MarkAllUserNotificationsReadResponse(
                        false, "Invalid workspace or user identifier", "INVALID_CLAIMS",
                        requestId, null, null, null);
                }

                var license = this.LicenseManager.GetLicense();
                var entity = await this[license!.DB!].MarkAllUserNotificationsReadAsync(
                    wid, uid,
                    ip, userAgent, deviceInfo, requestId,
                    reqLatitude, reqLongitude, cancellationToken);

                if (!entity.Success)
                {
                    return new MarkAllUserNotificationsReadResponse(
                        false, entity.Message ?? "Unable to mark all notifications as read",
                        entity.ErrorCode ?? "DB_ERROR", requestId, entity.Detail, null, null);
                }

                return new MarkAllUserNotificationsReadResponse(
                    Success:     true,
                    Message:     null,
                    ErrorCode:   string.Empty,
                    RequestId:   requestId,
                    Detail:      null,
                    MarkedCount: entity.MarkedCount,
                    UnreadCount: entity.UnreadCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MarkAllUserNotificationsReadAsync failed (req {req})", requestId);
                return new MarkAllUserNotificationsReadResponse(
                    false, ex.Message, "EXCEPTION", requestId, ex.GetType().Name, null, null);
            }
        }
    }
}

using ACS.Database.IDataAccess.NotificationsService.V1;
using ACS.License.V1;
using ACS.Models.Response.V1.NotificationsService.UserNotifications;
using ACS.Notifications.WebService.Services.V1.Interfaces;

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

                var license = this.LicenseManager.GetLicense();
                var entity = await this[license!.DB!].GetUserNotificationsAsync(
                    wid, uid, limit, offset, normalised,
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
    }
}

using ACS.Background;
using ACS.Database.IDataAccess.NotificationsService.V1;
using ACS.License.V1;
using ACS.Models.Response.V1.NotificationsService.Dashboard;
using ACS.Models.Response.V1.NotificationsService.UserNotifications;
using ACS.Notifications.WebService.Services.V1.Interfaces;
using ACS.Service.V1.Interfaces;
using System.Text.Json;
using ACS.Helper;

namespace ACS.Notifications.WebService.Services.V1.Services
{
    public class DashboardService(ILicenseManager licenseManager, IBackgroundTaskQueue backgroundTaskQueue, INotificationService notificationService, ILogger<DashboardService> logger) : Service.Service(licenseManager), IDashboardService
    {
        private IDashboardDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.NotificationsService.V1.DashboardDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in DashoardService.")
                };
            }
        }

        private IUserNotificationsDataAccess UserNotifs(Connection conn) => conn.Type switch
        {
            Database.IConnection.DatabaseType.PostgresDatabase =>
                Database.DataAccess.PostgresDataAccess.NotificationsService.V1.UserNotificationsDataAccess
                    .GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
            _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in DashoardService.")
        };

        public void StartNotify()
        {
            backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                try
                {
                    var license = this.LicenseManager.GetLicense();
                    this[license?.DB!].DashboardChanged+=(object? sender, JsonElement e) =>
                    {
                        var ev = e.Str("event");
                        var workspaceId = e.IntN("workspace_id");
                        var projectId = e.IntN("project_id");
                        var userId = e.IntN("user_id");
                        var data = e.TryGetProperty("data", out var d) ? d : default;
                        if (workspaceId== null ||  userId == null) return;
                        switch (ev)
                        {
                            case "notification_received":
                                object payload = new
                                {
                                    id = data.LngN("id"),
                                    title = data.Str("title"),
                                    message = data.Str("message"),
                                    is_read = false,
                                    priority = data.Str("priority"),
                                    action_url = data.Str("action_url"),
                                    action_label = data.Str("action_label"),
                                    related_entity_type = data.Str("related_entity_type"),
                                    related_entity_id = data.LngN("related_entity_id"),
                                    created_at = data.Str("created_at"),
                                };
                                notificationService.SendToUserGroup(ev, workspaceId.ToString()!, userId.ToString()!, payload);
                                break;
                            case "notification_read":
                                payload = new
                                {
                                    id = data.LngN("id"),
                                    read_at = data.Str("read_at"),
                                };
                                notificationService.SendToUserGroup(ev, workspaceId.ToString()!, userId.ToString()!, payload);
                                break;
                        }

                    }
                        ;
                    await this[license?.DB!].StartNotify(token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while listening to dashoard notification.");
                }
            });
        }

        public async Task<NotificationsDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDashboardAsync(wrokspace, user, cancellationToken);
            return new(
                Success: result.Success,
                Message: result.Message,
                ErrorCode: "",
                RequestId: requestId,
                GeneratedAt: result.GeneratedAt,
                Detail: result.Detail,
                Notifications: result.Notifications == null ? null : new (
                    UnreadCount: result.Notifications.UnreadCount,
                    Recent: result.Notifications.Recent ==  null ?[] : [.. result.Notifications.Recent.Select(recent => new NotificationItemResponse(
                          Id: recent.Id,
                          Title: recent.Title,
                          Message: recent.Message,
                          IsRead: recent.IsRead,
                          ActionUrl:recent.ActionUrl,
                          ActionLabel: recent.ActionLabel,
                          RelatedEntityType: recent.RelatedEntityType,
                          RelatedEntityId: recent.RelatedEntityId,
                          Priority: recent.Priority,
                          CreatedAt: recent.CreatedAt
                        ))]
                    )
                );
        }

        public async Task<UserNotificationsListResponse> GetUserNotificationsAsync(
            string workspace, string user, int limit, int offset, bool unreadOnly,
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

                var license = this.LicenseManager.GetLicense();
                var entity = await UserNotifs(license!.DB!).GetUserNotificationsAsync(
                    wid, uid, limit, offset, unreadOnly,
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
                var entity = await UserNotifs(license!.DB!).DeleteUserNotificationAsync(
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

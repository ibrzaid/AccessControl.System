using ACS.Background;
using ACS.Database.IDataAccess.NotificationsService.V1;
using ACS.License.V1;
using ACS.Models.Response.V1.NotificationsService.Dashboard;
using ACS.Models.Response.V1.NotificationsService.UserNotifications;
using ACS.Notifications.WebService.Services.V1.Interfaces;
using ACS.Service.V1.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using ACS.Helper;
using Npgsql;
using NpgsqlTypes;

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

        // ─────────────────────────────────────────────────────────────────────
        // Paginated user notifications (load-more) + delete
        //
        // The compiled DataAccess DLL only exposes the pre-existing dashboard
        // method, so these new endpoints talk to Postgres directly via Npgsql
        // using the connection details already on the licensed Connection.
        // ─────────────────────────────────────────────────────────────────────

        private static string BuildPgConnString(Connection conn) =>
            new NpgsqlConnectionStringBuilder
            {
                Host                   = string.IsNullOrEmpty(conn.Server) ? "localhost" : conn.Server,
                Port                   = conn.Port == 0 ? 5432 : conn.Port,
                Database               = conn.Name ?? string.Empty,
                Username               = conn.User ?? string.Empty,
                Password               = conn.Password ?? string.Empty,
                SslMode                = SslMode.Prefer,
                TrustServerCertificate = true,
                Timeout                = 15,
                CommandTimeout         = 30,
            }.ConnectionString;

        public async Task<UserNotificationsListResponse> GetUserNotificationsAsync(
            string workspace, string user, int limit, int offset, bool unreadOnly,
            string requestId, CancellationToken cancellationToken = default)
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
                var conn = license!.DB!;
                var cs = BuildPgConnString(conn);

                await using var db = new NpgsqlConnection(cs);
                await db.OpenAsync(cancellationToken);
                await using var cmd = new NpgsqlCommand(
                    "SELECT notifications_tbls_sch_v1.fun_get_user_notifications(@u, @w, @l, @o, @uo)", db);
                cmd.Parameters.Add(new NpgsqlParameter("u",  NpgsqlDbType.Integer) { Value = uid });
                cmd.Parameters.Add(new NpgsqlParameter("w",  NpgsqlDbType.Integer) { Value = wid });
                cmd.Parameters.Add(new NpgsqlParameter("l",  NpgsqlDbType.Integer) { Value = limit });
                cmd.Parameters.Add(new NpgsqlParameter("o",  NpgsqlDbType.Integer) { Value = offset });
                cmd.Parameters.Add(new NpgsqlParameter("uo", NpgsqlDbType.Boolean) { Value = unreadOnly });

                var raw = await cmd.ExecuteScalarAsync(cancellationToken);
                if (raw is null || raw is DBNull)
                {
                    return new UserNotificationsListResponse(
                        false, "Empty response from database", "DB_EMPTY", requestId,
                        null, null, null, limit, offset, 0, 0, false, []);
                }

                using var doc = JsonDocument.Parse(raw.ToString()!);
                var root = doc.RootElement;
                var success = root.TryGetProperty("success", out var s) && s.GetBoolean();
                if (!success)
                {
                    return new UserNotificationsListResponse(
                        false,
                        root.TryGetProperty("message", out var m) ? m.GetString() : "Unknown DB error",
                        "DB_ERROR", requestId,
                        null,
                        root.TryGetProperty("detail", out var d) ? d.GetString() : null,
                        null, limit, offset, 0, 0, false, []);
                }

                var items = new List<UserNotificationItem>();
                if (root.TryGetProperty("notifications", out var arr) && arr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var n in arr.EnumerateArray())
                    {
                        items.Add(new UserNotificationItem(
                            Id:                n.LngN("id") ?? 0,
                            Title:             n.Str("title") ?? string.Empty,
                            Message:           n.Str("message") ?? string.Empty,
                            IsRead:            n.TryGetProperty("is_read", out var ir) && ir.ValueKind == JsonValueKind.True,
                            ActionUrl:         n.Str("action_url"),
                            ActionLabel:       n.Str("action_label"),
                            RelatedEntityType: n.Str("related_entity_type"),
                            RelatedEntityId:   n.LngN("related_entity_id"),
                            Priority:          n.Str("priority"),
                            CreatedAt:         ParseDate(n, "created_at") ?? DateTime.UtcNow,
                            ReadAt:            ParseDate(n, "read_at")
                        ));
                    }
                }

                return new UserNotificationsListResponse(
                    Success:      true,
                    Message:      null,
                    ErrorCode:    string.Empty,
                    RequestId:    requestId,
                    GeneratedAt:  ParseDate(root, "generated_at"),
                    Detail:       null,
                    Timezone:     root.Str("timezone"),
                    Limit:        root.TryGetProperty("limit",  out var lj) && lj.TryGetInt32(out var li) ? li : limit,
                    Offset:       root.TryGetProperty("offset", out var oj) && oj.TryGetInt32(out var oi) ? oi : offset,
                    TotalCount:   root.LngN("total_count")  ?? 0,
                    UnreadCount:  root.LngN("unread_count") ?? 0,
                    HasMore:      root.TryGetProperty("has_more", out var hm) && hm.ValueKind == JsonValueKind.True,
                    Notifications: [.. items]);
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
            string requestId, CancellationToken cancellationToken = default)
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
                var conn = license!.DB!;
                var cs = BuildPgConnString(conn);

                await using var db = new NpgsqlConnection(cs);
                await db.OpenAsync(cancellationToken);
                await using var cmd = new NpgsqlCommand(
                    "SELECT notifications_tbls_sch_v1.fun_delete_user_notification(@u, @w, @nid)", db);
                cmd.Parameters.Add(new NpgsqlParameter("u",   NpgsqlDbType.Integer) { Value = uid });
                cmd.Parameters.Add(new NpgsqlParameter("w",   NpgsqlDbType.Integer) { Value = wid });
                cmd.Parameters.Add(new NpgsqlParameter("nid", NpgsqlDbType.Bigint)  { Value = notificationId });

                var raw = await cmd.ExecuteScalarAsync(cancellationToken);
                if (raw is null || raw is DBNull)
                {
                    return new DeleteUserNotificationResponse(
                        false, "Empty response from database", "DB_EMPTY",
                        requestId, null, null, null);
                }

                using var doc = JsonDocument.Parse(raw.ToString()!);
                var root = doc.RootElement;
                var success = root.TryGetProperty("success", out var s) && s.GetBoolean();
                if (!success)
                {
                    var detail = root.Str("detail");
                    var message = root.Str("message") ?? "Unable to delete notification";
                    return new DeleteUserNotificationResponse(
                        false, message,
                        detail == "NOT_FOUND" ? "NOT_FOUND" : "DB_ERROR",
                        requestId, detail, null, null);
                }

                return new DeleteUserNotificationResponse(
                    Success:     true,
                    Message:     null,
                    ErrorCode:   string.Empty,
                    RequestId:   requestId,
                    Detail:      null,
                    DeletedId:   root.LngN("deleted_id"),
                    UnreadCount: root.LngN("unread_count"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteUserNotificationAsync failed (req {req}, id {id})", requestId, notificationId);
                return new DeleteUserNotificationResponse(
                    false, ex.Message, "EXCEPTION", requestId, ex.GetType().Name, null, null);
            }
        }

        private static DateTime? ParseDate(JsonElement parent, string property)
        {
            if (!parent.TryGetProperty(property, out var p)) return null;
            if (p.ValueKind == JsonValueKind.Null || p.ValueKind == JsonValueKind.Undefined) return null;
            if (p.ValueKind == JsonValueKind.String && DateTime.TryParse(p.GetString(), out var dt))
                return DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            return null;
        }
    }
}

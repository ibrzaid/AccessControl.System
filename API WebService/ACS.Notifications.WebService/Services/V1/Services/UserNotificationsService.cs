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
            string? priority, string? type, string? since, string? until,
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

                // Whitelist priority/type code lists so we never forward
                // arbitrary client text into the SQL filter. Empty / missing
                // values pass through as null so the SQL skips the filter.
                static string? CleanCsv(string? raw, int maxItems = 16)
                {
                    if (string.IsNullOrWhiteSpace(raw)) return null;
                    var parts = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                   .Where(p => p.Length is > 0 and <= 32 &&
                                               p.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'))
                                   .Select(p => p.ToUpperInvariant())
                                   .Distinct()
                                   .Take(maxItems)
                                   .ToArray();
                    return parts.Length == 0 ? null : string.Join(',', parts);
                }
                var priorityClean = CleanCsv(priority);
                var typeClean     = CleanCsv(type);

                // Pass-through ISO-8601 timestamps; SQL casts text → timestamptz.
                static string? CleanIso(string? raw) =>
                    string.IsNullOrWhiteSpace(raw) ? null
                  : (raw!.Length > 64 ? raw[..64] : raw.Trim());
                var sinceClean = CleanIso(since);
                var untilClean = CleanIso(until);

                var license = this.LicenseManager.GetLicense();
                var entity = await this[license!.DB!].GetUserNotificationsAsync(
                    wid, uid, limit, offset, normalised, searchTerm,
                    priorityClean, typeClean, sinceClean, untilClean,
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
                    Type:              n.Type,
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

        public async Task<BulkDeleteUserNotificationsResponse> BulkDeleteUserNotificationsAsync(
            string workspace, string user, long[] notificationIds,
            string? ip, string? userAgent, string? deviceInfo, string requestId,
            decimal reqLatitude, decimal reqLongitude,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!int.TryParse(workspace, out var wid) || !int.TryParse(user, out var uid))
                {
                    return new BulkDeleteUserNotificationsResponse(
                        false, "Invalid workspace or user identifier", "INVALID_CLAIMS",
                        requestId, null, null, null, null, null);
                }

                if (notificationIds is null || notificationIds.Length == 0)
                {
                    return new BulkDeleteUserNotificationsResponse(
                        false, "No notification ids supplied", "EMPTY_INPUT",
                        requestId, "EMPTY_INPUT", 0, 0, Array.Empty<long>(), null);
                }

                var license = this.LicenseManager.GetLicense();
                var entity = await this[license!.DB!].BulkDeleteUserNotificationsAsync(
                    wid, uid, notificationIds,
                    ip, userAgent, deviceInfo, requestId,
                    reqLatitude, reqLongitude, cancellationToken);

                if (!entity.Success)
                {
                    return new BulkDeleteUserNotificationsResponse(
                        false, entity.Message ?? "Unable to bulk-delete notifications",
                        entity.ErrorCode ?? "DB_ERROR", requestId, entity.Detail,
                        entity.DeletedCount, entity.RequestedCount, entity.DeletedIds, entity.UnreadCount);
                }

                return new BulkDeleteUserNotificationsResponse(
                    Success:        true,
                    Message:        null,
                    ErrorCode:      string.Empty,
                    RequestId:      requestId,
                    Detail:         null,
                    DeletedCount:   entity.DeletedCount,
                    RequestedCount: entity.RequestedCount,
                    DeletedIds:     entity.DeletedIds,
                    UnreadCount:    entity.UnreadCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "BulkDeleteUserNotificationsAsync failed (req {req}, count {count})",
                    requestId, notificationIds?.Length ?? 0);
                return new BulkDeleteUserNotificationsResponse(
                    false, ex.Message, "EXCEPTION", requestId, ex.GetType().Name, null, null, null, null);
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

        /// <summary>
        /// Hosted-service entrypoint. Resolves the License → DB connection,
        /// dispatches to the postgres data access, and never throws.
        /// On exception the failure is logged here AND returned so the host can
        /// log a structured warning with the same correlation context.
        /// </summary>
        public async Task<(bool Success, string Detail)> RunDailyMaintenanceAsync(
            int readRetentionDays, int unreadRetentionDays,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var license = this.LicenseManager.GetLicense();
                if (license?.DB is null)
                {
                    logger.LogWarning("Daily maintenance skipped: no DB connection in license");
                    return (false, "No DB connection configured in license");
                }

                var entity = await this[license.DB].RunDailyMaintenanceAsync(
                    readRetentionDays, unreadRetentionDays, cancellationToken);

                if (!entity.Success)
                {
                    logger.LogWarning(
                        "Daily maintenance reported failure: {message} ({code}) — purged read={read} expired={exp} unread={unread}",
                        entity.Message, entity.ErrorCode,
                        entity.ReadPurgedCount, entity.ExpiredPurgedCount, entity.UnreadPurgedCount);
                }
                else
                {
                    logger.LogInformation(
                        "Daily maintenance OK in {elapsed}ms: read={read} expired={exp} unread={unread} total={total}",
                        entity.ElapsedMs, entity.ReadPurgedCount, entity.ExpiredPurgedCount,
                        entity.UnreadPurgedCount, entity.TotalPurgedCount);
                }

                var detail = $"read={entity.ReadPurgedCount ?? 0} expired={entity.ExpiredPurgedCount ?? 0} "
                           + $"unread={entity.UnreadPurgedCount ?? 0} total={entity.TotalPurgedCount ?? 0} "
                           + $"elapsed_ms={entity.ElapsedMs ?? 0}";
                return (entity.Success, detail);
            }
            catch (OperationCanceledException)
            {
                throw; // host shutdown — propagate so the BackgroundService loop exits cleanly
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RunDailyMaintenanceAsync threw");
                return (false, ex.GetType().Name + ": " + ex.Message);
            }
        }
    }
}

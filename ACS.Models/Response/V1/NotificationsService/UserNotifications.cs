#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.NotificationsService.UserNotifications
{
    /// <summary>
    /// Paginated list response for the notifications page (load-more flow).
    /// Backed by `notifications_tbls_sch_v1.fun_get_user_notifications`.
    /// </summary>
    public record UserNotificationsListResponse(
        bool Success,
        string? Message,
        string ErrorCode,
        string RequestId,
        [property: JsonPropertyName("generated_at")] DateTime? GeneratedAt,
        [property: JsonPropertyName("detail")] string? Detail,
        [property: JsonPropertyName("timezone")] string? Timezone,
        [property: JsonPropertyName("limit")] int Limit,
        [property: JsonPropertyName("offset")] int Offset,
        [property: JsonPropertyName("total_count")] long TotalCount,
        [property: JsonPropertyName("unread_count")] long UnreadCount,
        [property: JsonPropertyName("has_more")] bool HasMore,
        [property: JsonPropertyName("notifications")] UserNotificationItem[]? Notifications
    ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    public record UserNotificationItem(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("is_read")] bool IsRead,
        [property: JsonPropertyName("action_url")] string? ActionUrl,
        [property: JsonPropertyName("action_label")] string? ActionLabel,
        [property: JsonPropertyName("related_entity_type")] string? RelatedEntityType,
        [property: JsonPropertyName("related_entity_id")] long? RelatedEntityId,
        [property: JsonPropertyName("priority")] string? Priority,
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("read_at")] DateTime? ReadAt
    );

    /// <summary>
    /// Result of deleting a single notification.
    /// Backed by `notifications_tbls_sch_v1.fun_delete_user_notification`.
    /// </summary>
    public record DeleteUserNotificationResponse(
        bool Success,
        string? Message,
        string ErrorCode,
        string RequestId,
        [property: JsonPropertyName("detail")] string? Detail,
        [property: JsonPropertyName("deleted_id")] long? DeletedId,
        [property: JsonPropertyName("unread_count")] long? UnreadCount
    ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    /// <summary>
    /// Result of marking a single notification as read.
    /// Backed by `notifications_tbls_sch_v1.fun_mark_user_notification_read`.
    /// </summary>
    public record MarkUserNotificationReadResponse(
        bool Success,
        string? Message,
        string ErrorCode,
        string RequestId,
        [property: JsonPropertyName("detail")] string? Detail,
        [property: JsonPropertyName("notification_id")] long? NotificationId,
        [property: JsonPropertyName("was_unread")] bool? WasUnread,
        [property: JsonPropertyName("unread_count")] long? UnreadCount
    ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    /// <summary>
    /// Result of bulk mark-all-as-read.
    /// Backed by `notifications_tbls_sch_v1.fun_mark_all_user_notifications_read`.
    /// </summary>
    public record MarkAllUserNotificationsReadResponse(
        bool Success,
        string? Message,
        string ErrorCode,
        string RequestId,
        [property: JsonPropertyName("detail")] string? Detail,
        [property: JsonPropertyName("marked_count")] long? MarkedCount,
        [property: JsonPropertyName("unread_count")] long? UnreadCount
    ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    /// <summary>
    /// Request body for the bulk-delete endpoint. The server caps the array at
    /// 500 ids per call (enforced inside the SQL function).
    /// </summary>
    public record BulkDeleteUserNotificationsRequest(
        [property: JsonPropertyName("ids")] long[] Ids
    );

    /// <summary>
    /// Result of bulk-deleting a set of notifications owned by the calling user.
    /// Backed by `notifications_tbls_sch_v1.fun_bulk_delete_user_notifications`.
    /// </summary>
    public record BulkDeleteUserNotificationsResponse(
        bool Success,
        string? Message,
        string ErrorCode,
        string RequestId,
        [property: JsonPropertyName("detail")]          string? Detail,
        [property: JsonPropertyName("deleted_count")]   int?    DeletedCount,
        [property: JsonPropertyName("requested_count")] int?    RequestedCount,
        [property: JsonPropertyName("deleted_ids")]     long[]? DeletedIds,
        [property: JsonPropertyName("unread_count")]    long?   UnreadCount
    ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    /// <summary>
    /// Catalogue entry for a notification priority or type.  The
    /// <c>Labels</c> map is keyed by IETF locale (e.g. "en-US", "ar-SA")
    /// and is rendered directly by the preferences UI.
    /// </summary>
    public record NotificationCatalogItem(
        [property: JsonPropertyName("code")]   string Code,
        [property: JsonPropertyName("labels")] Dictionary<string, string>? Labels
    );

    /// <summary>
    /// Per-user notification preferences (Phase G).  Returned by
    /// GET /api/notifications/preferences and PUT /api/notifications/preferences.
    /// The "available_*" arrays carry the full active catalogue so the UI
    /// can render labels in the current locale without an extra round trip.
    /// </summary>
    public record UserNotificationPreferencesResponse(
        bool Success,
        string? Message,
        string ErrorCode,
        string RequestId,
        [property: JsonPropertyName("detail")]               string? Detail,
        [property: JsonPropertyName("muted_priorities")]     string[] MutedPriorities,
        [property: JsonPropertyName("muted_types")]          string[] MutedTypes,
        [property: JsonPropertyName("available_priorities")] NotificationCatalogItem[] AvailablePriorities,
        [property: JsonPropertyName("available_types")]      NotificationCatalogItem[] AvailableTypes
    ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    /// <summary>
    /// Request body for PUT /api/notifications/preferences.  Both arrays
    /// are required (send an empty array to clear all mutes for that
    /// dimension).  Server caps each array at 64 entries.
    /// </summary>
    public record UpdateUserNotificationPreferencesRequest(
        [property: JsonPropertyName("muted_priorities")] string[]? MutedPriorities,
        [property: JsonPropertyName("muted_types")]      string[]? MutedTypes
    );
}
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
}
#pragma warning restore IDE0130 // Namespace does not match folder structure

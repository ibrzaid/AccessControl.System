#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.NotificationsService.Dashboard
{
    public record NotificationsDashboardResponse(
   bool Success,
     string? Message,
     string ErrorCode,
     string RequestId,
    [property: JsonPropertyName("generated_at")] DateTime? GeneratedAt,
    [property: JsonPropertyName("detail")] string? Detail,
    [property: JsonPropertyName("notifications")] NotificationsResponse? Notifications
) : BaseResponses(Success, Message, ErrorCode, RequestId);

    public record NotificationsResponse(
        [property: JsonPropertyName("unread_count")] long UnreadCount,
        [property: JsonPropertyName("recent")] NotificationItemResponse[]? Recent
    );

    public record NotificationItemResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("is_read")] bool IsRead,
        [property: JsonPropertyName("action_url")] string? ActionUrl,
        [property: JsonPropertyName("action_label")] string? ActionLabel,
        [property: JsonPropertyName("related_entity_type")] string? RelatedEntityType,  // "camera" | "vehicle" | "parking_session" etc.
        [property: JsonPropertyName("related_entity_id")] long? RelatedEntityId,
        [property: JsonPropertyName("priority")] string? Priority,            // HIGH | MEDIUM | LOW
        [property: JsonPropertyName("created_at")] DateTime CreatedAt
    );
}
#pragma warning restore IDE0130 // Namespace does not match folder structure
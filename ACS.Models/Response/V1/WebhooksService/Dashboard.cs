
#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.WebhooksService.Dashboard
{
    public record WebhooksDashboardResponse(
   bool Success,
     string? Message,
     string ErrorCode,
     string RequestId,
    [property: JsonPropertyName("generated_at")] DateTime? GeneratedAt,
    [property: JsonPropertyName("detail")] string? Detail,
    [property: JsonPropertyName("webhooks")] WebhooksResponse? Webhooks    // null for non-admins
) : BaseResponses(Success, Message, ErrorCode, RequestId);

    public record WebhooksResponse(
        [property: JsonPropertyName("active_configs")] int ActiveConfigs,
        [property: JsonPropertyName("failed_24h")] long Failed24h,
        [property: JsonPropertyName("delivered_24h")] long Delivered24h,
        [property: JsonPropertyName("pending_now")] long PendingNow,
        [property: JsonPropertyName("health_pct")] decimal? HealthPct,      // null if no deliveries in 24h
        [property: JsonPropertyName("recent_failures")] WebhookFailureResponse[]? RecentFailures  // last 5 failed deliveries
    );

    public record WebhookFailureResponse(
        [property: JsonPropertyName("log_id")] long LogId,
        [property: JsonPropertyName("config_name")] string ConfigName,
        [property: JsonPropertyName("event_type")] string EventType,
        [property: JsonPropertyName("failure_reason")] string? FailureReason,
        [property: JsonPropertyName("retry_count")] int RetryCount,
        [property: JsonPropertyName("triggered_at")] DateTime TriggeredAt
    );
}
#pragma warning restore IDE0130 // Namespace does not match folder structure

#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.ParkingService.Dashboard
{
    public record ParkingDashboardResponse(
     bool Success,
     string? Message,
     string ErrorCode,
     string RequestId,
     [property: JsonPropertyName("module")] string? Module,
     [property: JsonPropertyName("enabled")] bool Enabled,
     [property: JsonPropertyName("generated_at")] DateTime? GeneratedAt,
     [property: JsonPropertyName("detail")] string? Detail,
     [property: JsonPropertyName("parking")] ParkingStatsResponse? Parking,
     [property: JsonPropertyName("projects_sessions")] ProjectSessionResponse[]? ProjectsSessions
) : BaseResponses(Success, Message, ErrorCode, RequestId);

    public record ParkingStatsResponse(
        [property: JsonPropertyName("active_sessions")] long ActiveSessions,
        [property: JsonPropertyName("revenue_today")] decimal RevenueToday,
        [property: JsonPropertyName("sessions_today")] long SessionsToday,
        [property: JsonPropertyName("sessions_closed_today")] long SessionsClosedToday,
        [property: JsonPropertyName("overstay_count")] long OverstayCount,
        [property: JsonPropertyName("avg_duration_minutes")] decimal? AvgDurationMinutes   // null if no closed sessions today
    );

    /// <summary>
    /// Per-project active session count — used to enrich project cards.
    /// </summary>
    public record ProjectSessionResponse(
        [property: JsonPropertyName("project_id")] long ProjectId,
        [property: JsonPropertyName("active_sessions")] long ActiveSessions
    );
}
#pragma warning restore IDE0130 // Namespace does not match folder structure

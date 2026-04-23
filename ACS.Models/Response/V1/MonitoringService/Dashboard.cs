#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.MonitoringService.Dashboard
{
    public record MonitoringDashboardResponse(
     bool Success,
     string? Message,
     string ErrorCode,
     string RequestId,
    [property: JsonPropertyName("generated_at")] DateTime? GeneratedAt,
    [property: JsonPropertyName("detail")] string? Detail,
    [property: JsonPropertyName("incidents")] IncidentsResponse? Incidents
 ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    public record IncidentsResponse(
        [property: JsonPropertyName("open_count")] long OpenCount,
        [property: JsonPropertyName("critical_count")] long CriticalCount,
        [property: JsonPropertyName("recent")] IncidentItemResponse[]? Recent,       // last 5 open incidents
        [property: JsonPropertyName("by_severity")] IncidentSeverityResponse[]? BySeverity    // CRITICAL | HIGH | MEDIUM | LOW counts
    );

    public record IncidentItemResponse(
        [property: JsonPropertyName("incident_id")] long IncidentId,
        [property: JsonPropertyName("alert_title")] string AlertTitle,
        [property: JsonPropertyName("severity")] string Severity,    // CRITICAL | HIGH | MEDIUM | LOW
        [property: JsonPropertyName("status")] string Status,      // OPEN | ACTIVE | ACKNOWLEDGED | IN_PROGRESS
        [property: JsonPropertyName("source")] string? Source,
        [property: JsonPropertyName("detected_at")] DateTime DetectedAt
    );

    public record IncidentSeverityResponse(
        [property: JsonPropertyName("severity")] string Severity,
        [property: JsonPropertyName("count")] long Count
    );
}
#pragma warning restore IDE0130 // Namespace does not match folder structure
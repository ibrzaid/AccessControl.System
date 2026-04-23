using System.Text.Json.Serialization;
using ACS.Models.Response.V1.ANPRService.Seach;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ACS.Models.Response.V1.ANPRService.Anpr
{
    public record AnprInsertResponse(
        bool Success,
        string? Message,
        string? ErrorCode,
        string RequestId,
        [property: JsonPropertyName("errors")] IReadOnlyList<string>? Errors,
        [property: JsonPropertyName("warnings")] IReadOnlyList<string>? Warnings,
        [property: JsonPropertyName("data")] AnprInsertDataResponse? Data,
        [property: JsonPropertyName("metadata")] AnprInsertMetaResponse? Metadata
    ) : BaseResponses(Success, Message, ErrorCode, RequestId);


    public record AnprInsertDataResponse(
       [property: JsonPropertyName("detection_id")] long? DetectionId,
       [property: JsonPropertyName("workspace_id")] long? WorkspaceId,
       [property: JsonPropertyName("plate")] string? Plate,
       [property: JsonPropertyName("detection_time")] DateTime? DetectionTime,
       [property: JsonPropertyName("confidence")] decimal? Confidence,
       [property: JsonPropertyName("coordinates")] AnprInsertCoordinatesResponse? Coordinates,
       [property: JsonPropertyName("warnings")] IReadOnlyList<string>? Warnings,
       [property: JsonPropertyName("detection")] AnprSearchDetectionResponse? Detection,
       [property: JsonPropertyName("signalr_payload")] AnprInsertSignalrResponse? SignalrPayload
   );

    public record AnprInsertCoordinatesResponse(
        [property: JsonPropertyName("lat")] decimal Lat,
        [property: JsonPropertyName("lng")] decimal Lng
    );

    public record AnprInsertSignalrResponse(
        [property: JsonPropertyName("channel")] string? Channel,
        [property: JsonPropertyName("event")] string? Event,
        [property: JsonPropertyName("workspace_id")] long? WorkspaceId,
        [property: JsonPropertyName("project_id")] long? ProjectId,
        [property: JsonPropertyName("timestamp")] long? Timestamp,
        [property: JsonPropertyName("data")] AnprSearchDetectionResponse? Data
    );

    public record AnprInsertMetaResponse(
        [property: JsonPropertyName("timestamp")] DateTime? Timestamp,
        [property: JsonPropertyName("audit_log_id")] long? AuditLogId,
        [property: JsonPropertyName("request_id")] string? RequestId,
        [property: JsonPropertyName("workspace_status")] AnprInsertWorkspaceStatusResponse? WorkspaceStatus
    );

    public record AnprInsertWorkspaceStatusResponse(
        [property: JsonPropertyName("current_records")] long? CurrentRecords,
        [property: JsonPropertyName("max_records")] long? MaxRecords
    );
}
#pragma warning restore IDE0130 // Namespace does not match folder structure
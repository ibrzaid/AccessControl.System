using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.ParkingService.Entry
{
    public class MetadataResponse
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("request_id")]
        public string? RequestId { get; set; }

        [JsonPropertyName("session_id")]
        public long SessionId { get; set; }

        [JsonPropertyName("entry_time")]
        public DateTime EntryTime { get; set; }

        [JsonPropertyName("vehicle_plate")]
        public string? VehiclePlate { get; set; }

        [JsonPropertyName("access_point_id")]
        public long AccessPointId { get; set; }

        [JsonPropertyName("project_id")]
        public int? ProjectId { get; set; }

        [JsonPropertyName("area_zone_id")]
        public int? AreaZoneId { get; set; }

        [JsonPropertyName("workspace_id")]
        public int? WorkspaceId { get; set; }

        [JsonPropertyName("subscriber_id")]
        public int? SubscriberId { get; set; }

        [JsonPropertyName("qr_code")]
        public string? QrCode { get; set; }

        [JsonPropertyName("qr_code_expiry")]
        public DateTime QrCodeExpiry { get; set; }

        [JsonPropertyName("audit_log_id")]
        public int AuditLogId { get; set; }
    }
}


using System.Text.Json;
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.Hardware
{
    public class HardwareItemResponse
    {
        [JsonPropertyName("hardware_id")] public long HardwareId { get; set; }
        [JsonPropertyName("access_point_id")] public long AccessPointId { get; set; }
        [JsonPropertyName("hardware_type_id")] public long HardwareTypeId { get; set; }
        [JsonPropertyName("hardware_status_id")] public long HardwareStatusId { get; set; }
        [JsonPropertyName("hardware_type_code")] public string? HardwareTypeCode { get; set; }
        [JsonPropertyName("hardware_type_names")] public Dictionary<string, string>? HardwareTypeNames { get; set; }
        [JsonPropertyName("hardware_status_names")] public Dictionary<string, string>? HardwareStatusNames { get; set; }
        [JsonPropertyName("parent_hardware_id")] public long? ParentHardwareId { get; set; }
        [JsonPropertyName("manufacturer")] public string? Manufacturer { get; set; }
        [JsonPropertyName("model")] public string? Model { get; set; }
        [JsonPropertyName("serial_number")] public string? SerialNumber { get; set; }
        [JsonPropertyName("firmware_version")] public string? FirmwareVersion { get; set; }
        [JsonPropertyName("hardware_status_code")] public string? StatusCode { get; set; }
        [JsonPropertyName("is_active")] public bool IsActive { get; set; }
        [JsonPropertyName("can_write")] public bool CanWrite { get; set; }
        [JsonPropertyName("last_activity")] public DateTime? LastActivity { get; set; }
        [JsonPropertyName("detection_count")] public long DetectionCount { get; set; }
        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("last_maintenance_date")] public DateTime? LastMaintenanceDate { get; set; }
        [JsonPropertyName("next_maintenance_date")] public DateTime? NextMaintenanceDate { get; set; }
        [JsonPropertyName("hardware_configuration")] public JsonElement? HardwareConfiguration { get; set; }
    }
}

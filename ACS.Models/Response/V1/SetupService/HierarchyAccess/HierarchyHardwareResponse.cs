
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.HierarchyAccess
{
    public class HierarchyHardwareResponse
    {
        [JsonPropertyName("hardware_id")]
        [Display(Name = "Hardware ID")]
        public int HardwareId { get; set; }

        [JsonPropertyName("hardware_type_code")]
        [Display(Name = "Type")]
        public string? HardwareTypeCode { get; set; }

        [JsonPropertyName("hardware_type_names")]
        [Display(Name = "Type Name")]
        public Dictionary<string, string>? HardwareTypeNames { get; set; }

        [JsonPropertyName("manufacturer")]
        [Display(Name = "Manufacturer")]
        public string? Manufacturer { get; set; }

        [JsonPropertyName("model")]
        [Display(Name = "Model")]
        public string? Model { get; set; }

        [JsonPropertyName("serial_number")]
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }

        [JsonPropertyName("firmware_version")]
        [Display(Name = "Firmware")]
        public string? FirmwareVersion { get; set; }

        [JsonPropertyName("hardware_status_code")]
        [Display(Name = "Status")]
        public string? HardwareStatusCode { get; set; }

        [JsonPropertyName("hardware_status_names")]
        [Display(Name = "Status Name")]
        public Dictionary<string, string>? HardwareStatusNames { get; set; }

        [JsonPropertyName("access_point_id")]
        [Display(Name = "Access Point")]
        public int? AccessPointId { get; set; }

        [JsonPropertyName("is_active")]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("last_activity")]
        [Display(Name = "Last Activity")]
        public DateTime? LastActivity { get; set; }

        [JsonPropertyName("detection_count")]
        [Display(Name = "Detection Count")]
        public int DetectionCount { get; set; }
    }
}

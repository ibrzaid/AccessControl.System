using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.SetupService.Hardware
{
    public class CreateHardwareRequest : BaseRequest
    {
        [JsonPropertyName("access_point_id")]
        [Required]
        [Range(1, int.MaxValue)]
        public int AccessPointId { get; set; }

        [JsonPropertyName("hardware_type_code")]
        [StringLength(50)]
        [Required]
        public string? HardwareTypeCode { get; set; }

        [JsonPropertyName("manufacturer")]
        [StringLength(100)]
        public string? Manufacturer { get; set; }

        [JsonPropertyName("model")]
        [StringLength(100)]
        public string? Model { get; set; }

        [JsonPropertyName("serial_number")]
        [StringLength(100)]
        [Required]
        public string? SerialNumber { get; set; }

        [JsonPropertyName("firmware_version")]
        [StringLength(50)]
        public string? FirmwareVersion { get; set; }

        [JsonPropertyName("hardware_status_code")]
        [StringLength(50)]
        public string? HardwareStatusCode { get; set; }


        [JsonPropertyName("last_maintenance_date")]
        public DateTime? LastMaintenanceDate { get; set; }


        [JsonPropertyName("next_maintenance_date")]
        public DateTime? NextMaintenanceDate { get; set; }


        [JsonPropertyName("hardware_configuration")]
        public string? HardwareConfiguration { get; set; }
    }
}

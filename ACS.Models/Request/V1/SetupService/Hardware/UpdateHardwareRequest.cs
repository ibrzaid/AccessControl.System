using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.SetupService.Hardware
{
    public class UpdateHardwareRequest : BaseRequest
    {
        [JsonPropertyName("hardware_type_code")]
        [StringLength(50)]
        public string? HardwareTypeCode { get; set; }

        [JsonPropertyName("manufacturer")]
        [StringLength(100)]
        public string? Manufacturer { get; set; }

        [JsonPropertyName("model")]
        [StringLength(100)]
        public string? Model { get; set; }

        [JsonPropertyName("serial_number")]
        [StringLength(100)]
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


        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
    }
}

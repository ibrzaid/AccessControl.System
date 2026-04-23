using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.SetupService.Hardware
{
    /// <summary>
    /// Request payload for connectivity / health-check test against a hardware
    /// asset. The persisted hardware record (if any) is identified by the route
    /// parameter; the body carries the candidate configuration so the operator
    /// can verify a draft (create / edit) without persisting it first.
    ///
    /// Intentionally does NOT inherit <c>BaseRequest</c>: a connectivity probe
    /// is not a geo-tagged write operation, so the latitude/longitude required
    /// fields would only force the frontend to gather GPS for a read-only
    /// device test.
    /// </summary>
    public class TestHardwareRequest
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

        [JsonPropertyName("hardware_configuration")]
        public string? HardwareConfiguration { get; set; }
    }
}

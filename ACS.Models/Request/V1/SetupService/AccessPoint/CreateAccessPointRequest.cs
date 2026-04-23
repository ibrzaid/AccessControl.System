using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.SetupService.AccessPoint
{
    public class CreateAccessPointRequest : BaseRequest
    {
        [JsonPropertyName("zone_id")]
        [Required]
        [Range(1, int.MaxValue)]
        public int ZoneId { get; set; }

        [JsonPropertyName("access_point_names")]
        [Required]
        [StringLength(200)]
        public string? AccessPointName { get; set; }

        [JsonPropertyName("prefix")]
        [StringLength(20)]
        public string? Prefix { get; set; }

        [JsonPropertyName("serial_number")]
        [Required]
        [StringLength(100)]
        public string? SerialNumber { get; set; }

        [JsonPropertyName("access_point_type_id")]
        [Required]
        public int? ApTypeId { get; set; }


        [JsonPropertyName("access_point_latitude")]
        public decimal? ApLatitude { get; set; }

        [JsonPropertyName("access_point_longitude")]
        public decimal? ApLongitude { get; set; }


        [JsonPropertyName("position_x")]
        public decimal? PositionX { get; set; }

        [JsonPropertyName("position_y")]
        public decimal? PositionY { get; set; }

        [JsonPropertyName("orientation_degrees")]
        public int? OrientationDegrees { get; set; }



    }
}

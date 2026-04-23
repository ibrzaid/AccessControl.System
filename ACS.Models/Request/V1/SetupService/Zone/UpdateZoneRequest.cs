
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.SetupService.Zone
{
    public class UpdateZoneRequest : BaseRequest
    {
        [JsonPropertyName("zone_names")]
        public Dictionary<string, string>? ZoneNames { get; set; }

        [JsonPropertyName("zone_code")]
        [StringLength(50)]
        public string? ZoneCode { get; set; }

        [JsonPropertyName("total_spots")]
        public int? TotalSpots { get; set; }

        [JsonPropertyName("grace_period")]
        public int? GracePeriod { get; set; }

        [JsonPropertyName("center_latitude")]
        public decimal? CenterLatitude { get; set; }

        [JsonPropertyName("center_longitude")]
        public decimal? CenterLongitude { get; set; }

        [JsonPropertyName("polygon_coordinates")]
        public object? PolygonCoordinates { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("floor_number")]
        public int? FloorNumber { get; set; }



        [JsonPropertyName("pin_new_parent_zone_id")]
        public int? NewParentZoneId { get; set; }
    }
}

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.SetupService.Zone
{
    public class CreateZoneRequest : BaseRequest
    {
        [JsonPropertyName("project_area_id")]
        [Required]
        [Range(1, int.MaxValue)]
        public int ProjectAreaId { get; set; }

        [JsonPropertyName("zone_names")]
        [Required(ErrorMessage = "Zone names are required")]
        public Dictionary<string, string> ZoneNames { get; set; } = [];

        [JsonPropertyName("zone_code")]
        [StringLength(50)]
        public string? ZoneCode { get; set; }

        [JsonPropertyName("zone_type_id")]
        public int? ZoneTypeId { get; set; }

        [JsonPropertyName("parent_zone_id")]
        public int? ParentZoneId { get; set; }

        [JsonPropertyName("access_level_id")]
        public int? AccessLevelId { get; set; }

        [JsonPropertyName("floor_number")]
        public int? FloorNumber { get; set; }

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
    }
}

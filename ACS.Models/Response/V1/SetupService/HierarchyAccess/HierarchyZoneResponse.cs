

using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.HierarchyAccess
{
    public class HierarchyZoneResponse
    {
        [JsonPropertyName("zone_id")]
        [Display(Name = "Zone ID")]
        public long ZoneId { get; set; }

        [JsonPropertyName("zone_code")]
        [Display(Name = "Zone Code")]
        public string ZoneCode { get; set; } = string.Empty;

        [JsonPropertyName("zone_names")]
        [Display(Name = "Zone Name")]
        public Dictionary<string, string>? ZoneNames { get; set; }

        [JsonPropertyName("zone_type_code")]
        [Display(Name = "Zone Type")]
        public string? ZoneTypeCode { get; set; }

        [JsonPropertyName("parent_zone_id")]
        [Display(Name = "Parent Zone")]
        public long? ParentZoneId { get; set; }

        [JsonPropertyName("depth")]
        [Display(Name = "Depth")]
        public int Depth { get; set; }

        [JsonPropertyName("is_active")]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("total_spots")]
        [Display(Name = "Total Spots")]
        public int TotalSpots { get; set; }

        [JsonPropertyName("available_spots")]
        [Display(Name = "Available Spots")]
        public int AvailableSpots { get; set; }

        [JsonPropertyName("grace_period")]
        [Display(Name = "Grace Period (min)")]
        public int GracePeriod { get; set; }

        [JsonPropertyName("center_latitude")]
        [Display(Name = "Latitude")]
        public decimal? CenterLatitude { get; set; }

        [JsonPropertyName("center_longitude")]
        [Display(Name = "Longitude")]
        public decimal? CenterLongitude { get; set; }

        [JsonPropertyName("polygon_coordinates")]
        [Display(Name = "Polygon")]
        public JsonElement? PolygonCoordinates { get; set; }

        [JsonPropertyName("has_children")]
        [Display(Name = "Has Children")]
        public bool HasChildren { get; set; }

        [JsonPropertyName("children")]
        [Display(Name = "Child Zones")]
        public List<HierarchyZoneResponse> Children { get; set; } = [];

        [JsonPropertyName("access_points")]
        [Display(Name = "Access Points")]
        public List<HierarchyAccessPointResponse> AccessPoints { get; set; } = [];
    }
}

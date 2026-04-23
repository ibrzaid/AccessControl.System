

using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.SetupService.HierarchyAccess
{
    public class HierarchyProjectAreaResponse
    {
        [JsonPropertyName("project_area_id")]
        [Display(Name = "Area ID")]
        public int ProjectAreaId { get; set; }

        [JsonPropertyName("project_area_names")]
        [Display(Name = "Area Name")]
        public Dictionary<string, string>? ProjectAreaNames { get; set; }

        [JsonPropertyName("total_spots")]
        [Display(Name = "Total Spots")]
        public int TotalSpots { get; set; }

        [JsonPropertyName("available_spots")]
        [Display(Name = "Available Spots")]
        public int AvailableSpots { get; set; }

        [JsonPropertyName("center_latitude")]
        [Display(Name = "Latitude")]
        public decimal? CenterLatitude { get; set; }

        [JsonPropertyName("center_longitude")]
        [Display(Name = "Longitude")]
        public decimal? CenterLongitude { get; set; }

        [JsonPropertyName("polygon_coordinates")]
        [Display(Name = "Polygon")]
        public JsonElement? PolygonCoordinates { get; set; }

        [JsonPropertyName("zones")]
        [Display(Name = "Zones")]
        public List<HierarchyZoneResponse> Zones { get; set; } = [];
    }
}

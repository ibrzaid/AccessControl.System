
using System.Text.Json.Serialization;

namespace ACS.Models.Request.V1.SetupService.Area
{
    public class UpdateAreaRequest : BaseRequest
    {
        [JsonPropertyName("project_area_names")]
        public Dictionary<string, string>? AreaNames { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("total_spots")]
        public int? TotalSpots { get; set; }

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
    }
}

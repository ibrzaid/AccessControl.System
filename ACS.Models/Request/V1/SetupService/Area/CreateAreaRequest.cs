using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.SetupService.Area
{
    public class CreateAreaRequest : BaseRequest
    {
        [JsonPropertyName("project_id")]
        [Required]
        [Range(1, int.MaxValue)]
        public int ProjectId { get; set; }

        [JsonPropertyName("project_area_names")]
        [Required(ErrorMessage = "Area names are required")]
        public Dictionary<string, string> AreaNames { get; set; } = [];

        [JsonPropertyName("area_type_id")]
        public int? AreaTypeId { get; set; }

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


        [JsonPropertyName("floor_number")]
        public int? FloorNumber { get; set; }
    }
}

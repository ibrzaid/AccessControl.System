

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.Area
{
    public class AreaItemResponse
    {
        [JsonPropertyName("project_area_id")] public int ProjectAreaId { get; set; }
        [JsonPropertyName("project_id")] public int ProjectId { get; set; }
        [JsonPropertyName("project_area_names")] public Dictionary<string, string>? AreaNames { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("total_spots")] public int? TotalSpots { get; set; }
        [JsonPropertyName("available_spots")] public int? AvailableSpots { get; set; }
        [JsonPropertyName("center_latitude")] public decimal? CenterLatitude { get; set; }
        [JsonPropertyName("center_longitude")] public decimal? CenterLongitude { get; set; }
        [JsonPropertyName("polygon_coordinates")] public JsonElement? PolygonCoords { get; set; }
        [JsonPropertyName("is_active")] public bool IsActive { get; set; }
        [JsonPropertyName("can_write")] public bool CanWrite { get; set; }
        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("area_type_id")] public long? AreaTypeId { get; set; }
        [JsonPropertyName("floor_number")] public long? FloorNumber { get; set; }
        [JsonPropertyName("area_type_code")] public string? AreaTypeCode { get; set; }
        [JsonPropertyName("area_type_names")] public Dictionary<string, string>? AreaTypeNames { get; set; }
    }
}

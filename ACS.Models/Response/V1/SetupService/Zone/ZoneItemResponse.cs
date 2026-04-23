

using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.Zone
{
    public class ZoneItemResponse
    {
        [JsonPropertyName("zone_id")] public int ZoneId { get; set; }
        [JsonPropertyName("project_area_id")] public int ProjectAreaId { get; set; }
        [JsonPropertyName("zone_code")] public string? ZoneCode { get; set; }
        [JsonPropertyName("zone_names")] public Dictionary<string, string>? ZoneNames { get; set; }
        [JsonPropertyName("parent_zone_id")] public int? ParentZoneId { get; set; }
        [JsonPropertyName("depth")] public int Depth { get; set; }
        [JsonPropertyName("total_spots")] public int? TotalSpots { get; set; }
        [JsonPropertyName("floor_number")] public int? FloorNumber { get; set; }
        [JsonPropertyName("available_spots")] public int? AvailableSpots { get; set; }
        [JsonPropertyName("zone_type_id")] public long? ZoneTypeId { get; set; }
        [JsonPropertyName("access_level_id")] public long? AccessLevelId { get; set; }
        [JsonPropertyName("access_level_code")] public string? AccessLevelCode { get; set; }
        [JsonPropertyName("access_level_names")] public Dictionary<string, string>? AccessLevelNames { get; set; }
        [JsonPropertyName("zone_type_code")] public string? ZoneTypeCode { get; set; }
        [JsonPropertyName("zone_type_names")] public Dictionary<string, string>? ZoneTypeNames { get; set; }
        [JsonPropertyName("grace_period")] public int? GracePeriod { get; set; }
        [JsonPropertyName("center_latitude")] public decimal? CenterLat { get; set; }
        [JsonPropertyName("center_longitude")] public decimal? CenterLon { get; set; }
        [JsonPropertyName("polygon_coordinates")] public object? Polygon { get; set; }
        [JsonPropertyName("has_children")] public bool HasChildren { get; set; }
        [JsonPropertyName("is_active")] public bool IsActive { get; set; }
        [JsonPropertyName("can_write")] public bool CanWrite { get; set; }
        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    }
}


using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.AccessPoint
{
    public class AccessPointItemResponse
    {
        [JsonPropertyName("access_point_id")] public int AccessPointId { get; set; }
        [JsonPropertyName("zone_id")] public int ZoneId { get; set; }
        [JsonPropertyName("access_point_name")] public Dictionary<string, string>? Name { get; set; }
        [JsonPropertyName("prefix")] public string? Prefix { get; set; }
        [JsonPropertyName("serial_number")] public string? SerialNumber { get; set; }
        [JsonPropertyName("access_point_type_id")] public int? ApTypeId { get; set; }
        [JsonPropertyName("access_level_id")] public int? AccessLevelId { get; set; }
        [JsonPropertyName("access_point_latitude")] public decimal? Latitude { get; set; }
        [JsonPropertyName("access_point_longitude")] public decimal? Longitude { get; set; }
        [JsonPropertyName("access_point_type_code")] public string? AccessPointTypeCode { get; set; }
        [JsonPropertyName("access_point_type_names")] public Dictionary<string, string>? AccessPointTypeNames { get; set; }
        [JsonPropertyName("position_x")] public decimal? PositionX { get; set; }
        [JsonPropertyName("position_y")] public decimal? PositionY { get; set; }
        [JsonPropertyName("orientation_degrees")] public long? OrientationDegrees { get; set; }
        [JsonPropertyName("is_active")] public bool IsActive { get; set; }
        [JsonPropertyName("can_write")] public bool CanWrite { get; set; }
        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    }
}

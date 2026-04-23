using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService
{
    public class AreaZoneResponse: BaseResponse
    {
        [JsonPropertyName("zone_id")]
        public int ZoneId { get; set; }

        [JsonPropertyName("zone_code")]
        public string? ZoneCode { get; set; }

        [JsonPropertyName("zone_names")]
        public Dictionary<string, string>? ZoneNames { get; set; }

        [JsonPropertyName("project_id")]
        public int? ProjectId { get; set; }

        [JsonPropertyName("project_area_id")]
        public int? ProjectAreaId { get; set; }

        [JsonPropertyName("parent_zone_id")]
        public int? ParentZoneId { get; set; }

        [JsonPropertyName("zone_type_id")]
        public int? ZoneTypeId { get; set; }

        [JsonPropertyName("floor_number")]
        public int? FloorNumber { get; set; }

        [JsonPropertyName("total_spots")]
        public int? TotalSpots { get; set; }

        [JsonPropertyName("available_spots")]
        public int? AvailableSpots { get; set; }

        [JsonPropertyName("center_latitude")]
        public decimal? CenterLatitude { get; set; }

        [JsonPropertyName("center_longitude")]
        public decimal? CenterLongitude { get; set; }

        [JsonPropertyName("access_level_id")]
        public int? AccessLevelId { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}

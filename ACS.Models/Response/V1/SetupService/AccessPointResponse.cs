
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService
{
    public class AccessPointResponse: BaseResponse
    {

        [JsonPropertyName("access_point_id")]
        public long AccessPointId { get; set; }

        [JsonPropertyName("access_point_name")]
        public Dictionary<string, string>? AccessPointName { get; set; }

        [JsonPropertyName("access_point_type_code")]
        public string? AccessPointTypeCode { get; set; }

        [JsonPropertyName("access_point_type_names")]
        public Dictionary<string, string>? AccessPointTypeNames { get; set; }

        [JsonPropertyName("position_x")]
        public decimal? PositionX { get; set; }

        [JsonPropertyName("position_y")]
        public decimal? PositionY { get; set; }

        [JsonPropertyName("orientation_degrees")]
        public decimal? OrientationDegrees { get; set; }

        [JsonPropertyName("access_point_latitude")]
        public decimal? AccessPointLatitude { get; set; }

        [JsonPropertyName("access_point_longitude")]
        public decimal? AccessPointLongitude { get; set; }

        [JsonPropertyName("zone_id")]
        public int? ZoneId { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}

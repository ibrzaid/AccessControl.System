using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsDatumResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }

        [JsonPropertyName("user_id")]
        public long? UserId { get; set; }

        [JsonPropertyName("user_ip")]
        public object? UserIp { get; set; }

        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("user_name")]
        public string? UserName { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("new_values")]
        public object? NewValues { get; set; }

        [JsonPropertyName("old_values")]
        public UserLogsOldValuesResponse? OldValues { get; set; }

        [JsonPropertyName("project_id")]
        public object? ProjectId { get; set; }

        [JsonPropertyName("request_id")]
        public object? RequestId { get; set; }

        [JsonPropertyName("user_agent")]
        public object? UserAgent { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("entity_type")]
        public string? EntityType { get; set; }

        [JsonPropertyName("area_zone_id")]
        public object? AreaZoneId { get; set; }

        [JsonPropertyName("resource_path")]
        public object? ResourcePath { get; set; }

        [JsonPropertyName("access_point_id")]
        public object? AccessPointId { get; set; }

        [JsonPropertyName("project_area_id")]
        public object? ProjectAreaId { get; set; }
    }
}

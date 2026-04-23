using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserAccess
{
    public class ActivityLogResponse
    {
        [JsonPropertyName("log_id")]            public int      LogId           { get; set; }
        [JsonPropertyName("user_id")]           public int?     UserId          { get; set; }
        [JsonPropertyName("action")]            public string   Action          { get; set; } = string.Empty;
        [JsonPropertyName("entity_type")]       public string   EntityType      { get; set; } = string.Empty;
        [JsonPropertyName("resource_name")]     public string?  ResourceName    { get; set; }
        [JsonPropertyName("description")]       public string?  Description     { get; set; }
        [JsonPropertyName("resource_path")]     public string?  ResourcePath    { get; set; }
        [JsonPropertyName("request_id")]        public string?  RequestId       { get; set; }
        [JsonPropertyName("ip_address")]        public string?  IpAddress       { get; set; }
        [JsonPropertyName("user_agent")]        public string?  UserAgent       { get; set; }
        [JsonPropertyName("performed_by")]      public int?     PerformedBy     { get; set; }
        [JsonPropertyName("performed_by_name")] public string?  PerformedByName { get; set; }
        [JsonPropertyName("old_values")]        public object?  OldValues       { get; set; }
        [JsonPropertyName("new_values")]        public object?  NewValues       { get; set; }
        [JsonPropertyName("project_id")]        public int?     ProjectId       { get; set; }
        [JsonPropertyName("project_area_id")]   public int?     ProjectAreaId   { get; set; }
        [JsonPropertyName("zone_id")]           public int?     ZoneId          { get; set; }
        [JsonPropertyName("access_point_id")]   public int?     AccessPointId   { get; set; }
        [JsonPropertyName("latitude")]          public decimal? Latitude        { get; set; }
        [JsonPropertyName("longitude")]         public decimal? Longitude       { get; set; }
        [JsonPropertyName("created_at")]        public DateTime CreatedAt       { get; set; }
    }
}

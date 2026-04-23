using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService
{
    public class ProjectResponse: BaseResponse
    {
        [JsonPropertyName("project_id")]
        public int ProjectId { get; set; }

        [JsonPropertyName("project_names")]
        public Dictionary<string, string>? ProjectNames { get; set; }

        [JsonPropertyName("project_description")]
        public string? ProjectDescription { get; set; }

        [JsonPropertyName("project_address")]
        public string? ProjectAddress { get; set; }

        [JsonPropertyName("project_city")]
        public string? ProjectCity { get; set; }

        [JsonPropertyName("project_state")]
        public string? ProjectState { get; set; }

        [JsonPropertyName("country_id")]
        public int? CountryId { get; set; }

        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("project_latitude")]
        public decimal? ProjectLatitude { get; set; }

        [JsonPropertyName("project_longitude")]
        public decimal? ProjectLongitude { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("project_is_public")]
        public bool? ProjectIsPublic { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}

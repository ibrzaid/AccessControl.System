

using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.Project
{
    public class ProjectItemResponse
    {
        [JsonPropertyName("project_id")] public long ProjectId { get; set; }
        [JsonPropertyName("project_names")] public Dictionary<string, string>? Names { get; set; }
        [JsonPropertyName("project_description")] public string? Description { get; set; }
        [JsonPropertyName("project_type_id")] public long? ProjectTypeId { get; set; }
        [JsonPropertyName("project_type_name")] public Dictionary<string, string>? ProjectTypeNames { get; set; }
        [JsonPropertyName("project_type_code")] public string? ProjectTypeCode { get; set; }
        [JsonPropertyName("project_address")] public string? Address { get; set; }
        [JsonPropertyName("project_city")] public string? City { get; set; }
        [JsonPropertyName("project_state")] public string? State { get; set; }
        [JsonPropertyName("country_id")] public long? CountryId { get; set; }
        [JsonPropertyName("country_name")] public Dictionary<string, string>? CountryNames { get; set; }
        [JsonPropertyName("postal_code")] public string? PostalCode { get; set; }
        [JsonPropertyName("project_latitude")] public decimal? Latitude { get; set; }
        [JsonPropertyName("project_longitude")] public decimal? Longitude { get; set; }
        [JsonPropertyName("timezone")] public string? Timezone { get; set; }
        [JsonPropertyName("project_is_public")] public bool IsPublic { get; set; }
        [JsonPropertyName("is_active")] public bool IsActive { get; set; }
        [JsonPropertyName("can_write")] public bool CanWrite { get; set; }
        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    }
}

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.AccessLevel
{
    public class AccessLevelResponse
    {
        [JsonPropertyName("access_level_id")]
        [Display(Name = "Access Level ID")]
        public int AccessLevelId { get; set; }

        [JsonPropertyName("access_level_code")]
        [Display(Name = "Code")]
        public string AccessLevelCode { get; set; } = string.Empty;

        [JsonPropertyName("access_level_names")]
        [Display(Name = "Name")]
        public object? AccessLevelNames { get; set; }

        [JsonPropertyName("priority")]
        [Display(Name = "Priority")]
        public int Priority { get; set; }

        [JsonPropertyName("description")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}

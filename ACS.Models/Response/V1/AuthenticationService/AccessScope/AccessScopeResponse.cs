using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.AccessScope
{
    public class AccessScopeResponse
    {
        [JsonPropertyName("scope_type_id")]
        [Display(Name = "Scope Type ID")]
        public int ScopeTypeId { get; set; }

        [JsonPropertyName("scope_type_code")]
        [Display(Name = "Scope Code")]
        public string ScopeTypeCode { get; set; } = string.Empty;

        [JsonPropertyName("scope_name")]
        [Display(Name = "Scope Name")]
        public object? ScopeName { get; set; }

        [JsonPropertyName("scope_level")]
        [Display(Name = "Scope Level")]
        public int ScopeLevel { get; set; }

        [JsonPropertyName("description")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}

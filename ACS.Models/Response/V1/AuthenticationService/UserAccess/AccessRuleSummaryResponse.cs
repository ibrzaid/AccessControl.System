using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserAccess
{
    public class AccessRuleSummaryResponse
    {
        [JsonPropertyName("user_access_id")]
        [Display(Name = "Access Rule ID")]
        public long UserAccessId { get; set; }

        [JsonPropertyName("scope_code")]
        [Display(Name = "Scope")]
        public string ScopeCode { get; set; } = string.Empty;

        [JsonPropertyName("scope_level")]
        [Display(Name = "Scope Level")]
        public int ScopeLevel { get; set; }

        [JsonPropertyName("access_level_code")]
        [Display(Name = "Access Level")]
        public string AccessLevelCode { get; set; } = string.Empty;

        [JsonPropertyName("is_admin_scope")]
        [Display(Name = "Admin Scope")]
        public bool IsAdminScope { get; set; }

        [JsonPropertyName("is_inherited")]
        [Display(Name = "Inherited")]
        public bool IsInherited { get; set; }

        [JsonPropertyName("project_id")]
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }

        [JsonPropertyName("project_area_id")]
        [Display(Name = "Project Area")]
        public int? ProjectAreaId { get; set; }

        [JsonPropertyName("zone_id")]
        [Display(Name = "Zone")]
        public int? ZoneId { get; set; }

        [JsonPropertyName("access_point_id")]
        [Display(Name = "Access Point")]
        public int? AccessPointId { get; set; }

        [JsonPropertyName("resource_name")]
        [Display(Name = "Resource")]
        public string? ResourceName { get; set; }

        [JsonPropertyName("expires_at")]
        [Display(Name = "Expires At")]
        public DateTime? ExpiresAt { get; set; }

        [JsonPropertyName("granted_at")]
        [Display(Name = "Granted At")]
        public DateTime GrantedAt { get; set; }


        [JsonPropertyName("scope_type_id")]
        [Display(Name = "Scope Type Id")]
        public long ScopeTypeId { get; set; }



        [JsonPropertyName("scope_type_code")]
        [Display(Name = "Scope Type Code")]
        public string ScopeTypeCode { get; set; } = string.Empty;
    }
}

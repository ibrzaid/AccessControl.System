

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.AccessRule
{
    public class AccessRuleResponse
    {
        [JsonPropertyName("user_access_id")]
        [Display(Name = "Access Rule ID")]
        public long UserAccessId { get; set; }

        [JsonPropertyName("user_id")]
        [Display(Name = "User ID")]
        public int UserId { get; set; }

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

        [JsonPropertyName("visibility_only")]
        [Display(Name = "Visibility Only")]
        public bool VisibilityOnly { get; set; }

        [JsonPropertyName("is_inherited")]
        [Display(Name = "Inherited")]
        public bool IsInherited { get; set; }

        [JsonPropertyName("is_exclusive")]
        [Display(Name = "Exclusive")]
        public bool IsExclusive { get; set; }

        [JsonPropertyName("inherit_down_to")]
        [Display(Name = "Inherit Down To")]
        public int? InheritDownTo { get; set; }

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

        [JsonPropertyName("granted_by")]
        [Display(Name = "Granted By")]
        public int? GrantedBy { get; set; }

        [JsonPropertyName("notes")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }


        [JsonPropertyName("user")]
        [Display(Name = "User")]
        public AccessUserResponse? User { get; set; }
    }
}

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.AuthenticationService.AccessUser
{
    public class GrantAccessRequest : BaseRequest
    {
        [JsonPropertyName("user_id")]
        [Required(ErrorMessage = "Please select a user")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid user")]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [JsonPropertyName("scope_type_id")]
        [Required(ErrorMessage = "Please select a scope type")]
        [Range(1, 5, ErrorMessage = "Scope type must be between 1 and 5")]
        [Display(Name = "Scope Type")]
        public int ScopeTypeId { get; set; }

        [JsonPropertyName("access_level_id")]
        [Required(ErrorMessage = "Please select an access level")]
        [Range(1, 5, ErrorMessage = "Access level must be between 1 and 5")]
        [Display(Name = "Access Level")]
        public int AccessLevelId { get; set; }

        [JsonPropertyName("project_id")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid project")]
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }

        [JsonPropertyName("project_area_id")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid project area")]
        [Display(Name = "Project Area")]
        public int? ProjectAreaId { get; set; }

        [JsonPropertyName("zone_id")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid zone")]
        [Display(Name = "Zone")]
        public int? ZoneId { get; set; }

        [JsonPropertyName("access_point_id")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid access point")]
        [Display(Name = "Access Point")]
        public int? AccessPointId { get; set; }

        [JsonPropertyName("is_admin_scope")]
        [Display(Name = "Admin Scope")]
        public bool IsAdminScope { get; set; } = false;

        [JsonPropertyName("is_inherited")]
        [Display(Name = "Auto-Inheritance")]
        public bool IsInherited { get; set; } = true;

        [JsonPropertyName("expires_at")]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiresAt { get; set; }

        [JsonPropertyName("notes")]
        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}

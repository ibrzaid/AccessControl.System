using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;



namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class ManageableUserResponse
    {
        [JsonPropertyName("user_id")]
        [Display(Name = "User ID")]
        public int UserId { get; set; }

        [JsonPropertyName("full_name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("avatar_url")]
        [Display(Name = "Avatar URL")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("scope_type_code")]
        [Display(Name = "Scope")]
        public string ScopeTypeCode { get; set; } = string.Empty;

        [JsonPropertyName("resource_id")]
        [Display(Name = "Resource ID")]
        public long ResourceId { get; set; }

        [JsonPropertyName("access_level_code")]
        [Display(Name = "Access Level")]
        public string AccessLevelCode { get; set; } = string.Empty;

        [JsonPropertyName("is_admin_scope")]
        [Display(Name = "Admin Scope")]
        public bool IsAdminScope { get; set; }
    }
}

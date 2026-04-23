using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using ACS.Models.Response.V1.AuthenticationService.UserRole;
using ACS.Models.Response.V1.AuthenticationService.UserAccess;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class UserResponse
    {
        [JsonPropertyName("user_id")]
        [Display(Name = "User ID")]
        public int UserId { get; set; }

        [JsonPropertyName("workspace_id")]
        [Display(Name = "Workspace ID")]
        public int WorkspaceId { get; set; }

        [JsonPropertyName("username")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("full_name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("phone_number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("avatar_url")]
        [Display(Name = "Avatar URL")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("department")]
        [Display(Name = "Department")]
        public string? Department { get; set; }

        [JsonPropertyName("job_title")]
        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [JsonPropertyName("timezone")]
        [Display(Name = "Timezone")]
        public string Timezone { get; set; } = "UTC";

        [JsonPropertyName("language")]
        [Display(Name = "Language")]
        public string Language { get; set; } = "en-US";

        [JsonPropertyName("mfa_enabled")]
        [Display(Name = "MFA Enabled")]
        public bool MfaEnabled { get; set; }

        [JsonPropertyName("notification_preferences")]
        [Display(Name = "Notification Preferences")]
        public object? NotificationPreferences { get; set; }

        [JsonPropertyName("last_login")]
        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }

        [JsonPropertyName("user_status_code")]
        [Display(Name = "Status")]
        public string UserStatusCode { get; set; } = string.Empty;

        [JsonPropertyName("user_status_name")]
        [Display(Name = "Status Name")]
        public object? UserStatusName { get; set; }

        [JsonPropertyName("failed_login_attempts")]
        [Display(Name = "Failed Login Attempts")]
        public int FailedLoginAttempts { get; set; }

        [JsonPropertyName("locked_until")]
        [Display(Name = "Locked Until")]
        public DateTime? LockedUntil { get; set; }

        [JsonPropertyName("password_changed_at")]
        [Display(Name = "Password Changed At")]
        public DateTime? PasswordChangedAt { get; set; }

        [JsonPropertyName("created_at")]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("scope_type_code")]
        [Display(Name = "Scope")]
        public string? ScopeTypeCode { get; set; }

        [JsonPropertyName("roles")]
        [Display(Name = "Roles")]
        public List<RoleSummaryResponse> Roles { get; set; } = [];

        [JsonPropertyName("access_rules")]
        [Display(Name = "Access Rules")]
        public List<AccessRuleSummaryResponse>? AccessRules{get; set; }

        [JsonPropertyName("manager_user_id")] public long? ManagerUerId { get; set; }
        [JsonPropertyName("manager")] public ManagerUserResponse? Manager { get; set; }
    }
}

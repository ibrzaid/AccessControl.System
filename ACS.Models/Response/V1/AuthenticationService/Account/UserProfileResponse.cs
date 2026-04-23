using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.Account
{
    public class UserProfileResponse
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("job_title")]
        public string? JobTitle { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("last_login")]
        public DateTime? LastLogin { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("workspace_id")]
        public int WorkspaceId { get; set; }

        [JsonPropertyName("workspace_name")]
        public string? WorkspaceName { get; set; }


        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }


        [JsonPropertyName("mfa_enabled")]
        public bool MfaEnabled { get; set; }
    }
}

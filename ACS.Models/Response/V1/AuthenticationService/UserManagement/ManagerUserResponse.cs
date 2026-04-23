

using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class ManagerUserResponse
    {
        [JsonPropertyName("manager_user_id")]
        public long? ManagerUserId { get; set; }

        [JsonPropertyName("manager_full_name")]
        public string? ManagerFullName { get; set; }

        [JsonPropertyName("manager_email")]
        public string? ManagerEmail { get; set; }

        [JsonPropertyName("manager_username")]
        public string? ManagerUsername { get; set; }

        [JsonPropertyName("manager_avatar_url")]
        public string? ManagerAvatarUrl { get; set; }

        [JsonPropertyName("assigned_at")]
        public DateTime? AssignedAt { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes
        {
            get; set;
        }
    }
}

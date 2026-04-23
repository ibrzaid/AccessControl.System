
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.Account
{
    public class UserSessionResponse
    {
        [JsonPropertyName("session_id")]
        public Guid SessionId { get; set; }

        [JsonPropertyName("user_id")]
        public long? UserId { get; set; }

        [JsonPropertyName("workspace_id")]
        public int? WorkspaceId { get; set; }

        [JsonPropertyName("token_expires_at")]
        public DateTime? TokenExpiresAt { get; set; }

        [JsonPropertyName("refresh_token_expires_at")]
        public DateTime? RefreshTokenExpiresAt { get; set; }

        [JsonPropertyName("ip_address")]
        public string? IpAddress { get; set; }

        [JsonPropertyName("user_agent")]
        public string? UserAgent { get; set; }

        [JsonPropertyName("device_info")]
        public string? DeviceInfo { get; set; }

        [JsonPropertyName("client_id")]
        public string? ClientId { get; set; }

        [JsonPropertyName("client_version")]
        public string? ClientVersion { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("last_accessed_at")]
        public DateTime? LastAccessedAt { get; set; }

        [JsonPropertyName("logout_at")]
        public DateTime? LogoutAt { get; set; }

        [JsonPropertyName("latitude")]
        public decimal? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public decimal? Longitude { get; set; }

        [JsonPropertyName("failed_refresh_attempts")]
        public int? FailedRefreshAttempts { get; set; }
    }
}

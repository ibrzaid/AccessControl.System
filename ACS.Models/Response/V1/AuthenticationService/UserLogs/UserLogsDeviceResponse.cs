using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsDeviceResponse
    {
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }

        [JsonPropertyName("client_id")]
        public string? ClientId { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("logout_at")]
        public DateTime? LogoutAt { get; set; }

        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("ip_address")]
        public string? IpAddress { get; set; }

        [JsonPropertyName("device_info")]
        public UserLogsDeviceInfoResponse? DeviceInfo { get; set; }

        [JsonPropertyName("client_version")]
        public object? ClientVersion { get; set; }

        [JsonPropertyName("last_accessed_at")]
        public DateTime LastAccessedAt { get; set; }

        [JsonPropertyName("failed_refresh_attempts")]
        public int FailedRefreshAttempts { get; set; }
    }
}

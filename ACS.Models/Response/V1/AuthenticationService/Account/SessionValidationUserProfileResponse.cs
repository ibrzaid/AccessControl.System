
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.Account
{
    public class SessionValidationUserProfileResponse : UserProfileResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("session_id")]
        public Guid SessionId { get; set; }

        [JsonPropertyName("token_expires_at")]
        public DateTime TokenExpiresAt { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;
    }
}

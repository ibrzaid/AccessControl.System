using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.Account
{
    public class SessionValidationResponse : BaseResponse
    {

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("user_data")]
        public SessionValidationUserProfileResponse? UserData { get; set; }

        [JsonPropertyName("expires_in_seconds")]
        public double ExpiresInSeconds { get; set; }
    }
}

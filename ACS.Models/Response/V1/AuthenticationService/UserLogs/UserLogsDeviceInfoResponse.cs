using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsDeviceInfoResponse
    {
        [JsonPropertyName("os")]
        public string? Os { get; set; }

        [JsonPropertyName("browser")]
        public string? Browser { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsTopPathResponse
    {
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}

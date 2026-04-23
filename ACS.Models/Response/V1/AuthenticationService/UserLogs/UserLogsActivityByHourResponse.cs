using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsActivityByHourResponse
    {
        [JsonPropertyName("hour")]
        public int Hour { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}

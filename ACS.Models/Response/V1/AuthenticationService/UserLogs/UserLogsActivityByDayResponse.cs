using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsActivityByDayResponse
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}

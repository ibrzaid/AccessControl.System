using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsOverviewResponse
    {
        [JsonPropertyName("active_days")]
        public int ActiveDays { get; set; }

        [JsonPropertyName("total_events")]
        public int TotalEvents { get; set; }

        [JsonPropertyName("last_activity")]
        public DateTime? LastActivity { get; set; }

        [JsonPropertyName("first_activity")]
        public DateTime? FirstActivity { get; set; }
    }
}

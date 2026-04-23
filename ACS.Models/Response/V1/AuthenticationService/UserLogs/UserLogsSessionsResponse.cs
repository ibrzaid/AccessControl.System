using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsSessionsResponse
    {
        [JsonPropertyName("devices")]
        public List<UserLogsDeviceResponse>? Devices { get; set; }

        [JsonPropertyName("unique_ips")]
        public int? UniqueIps { get; set; }

        [JsonPropertyName("first_session")]
        public DateTime? FirstSession { get; set; }

        [JsonPropertyName("latest_session")]
        public DateTime? LatestSession { get; set; }

        [JsonPropertyName("total_sessions")]
        public int? TotalSessions { get; set; }

        [JsonPropertyName("unique_clients")]
        public int? UniqueClients { get; set; }

        [JsonPropertyName("active_sessions")]
        public int? ActiveSessions { get; set; }

        [JsonPropertyName("closed_sessions")]
        public int? ClosedSessions { get; set; }

        [JsonPropertyName("last_session_activity")]
        public DateTime? LastSessionActivity { get; set; }

        [JsonPropertyName("total_failed_refreshes")]
        public int? TotalFailedRefreshes { get; set; }
    }
}

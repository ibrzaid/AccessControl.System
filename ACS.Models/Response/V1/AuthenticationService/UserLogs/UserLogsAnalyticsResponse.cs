using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsAnalyticsResponse
    {
        [JsonPropertyName("login")]
        public UserLogsLoginResponse? Login { get; set; }

        [JsonPropertyName("network")]
        public UserLogsNetworkResponse? Network { get; set; }

        [JsonPropertyName("overview")]
        public UserLogsOverviewResponse? Overview { get; set; }

        [JsonPropertyName("sessions")]
        public UserLogsSessionsResponse? Sessions { get; set; }

        [JsonPropertyName("top_paths")]
        public List<UserLogsTopPathResponse>? TopPaths { get; set; }

        [JsonPropertyName("management")]
        public UserLogsManagementResponse? Management { get; set; }

        [JsonPropertyName("activity_by_day")]
        public List<UserLogsActivityByDayResponse>? ActivityByDay { get; set; }

        [JsonPropertyName("activity_by_hour")]
        public List<UserLogsActivityByHourResponse>? ActivityByHour { get; set; }

        [JsonPropertyName("actions_breakdown")]
        public List<UserLogsActionsBreakdownResponse>? ActionsBreakdown { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsLoginResponse
    {
        [JsonPropertyName("last_login")]
        public DateTime? LastLogin { get; set; }

        [JsonPropertyName("first_login")]
        public DateTime? FirstLogin { get; set; }

        [JsonPropertyName("total_logins")]
        public int? TotalLogins { get; set; }

        [JsonPropertyName("failed_logins")]
        public int? FailedLogins { get; set; }

        [JsonPropertyName("total_logouts")]
        public int? TotalLogouts { get; set; }

        [JsonPropertyName("total_refreshes")]
        public int? TotalRefreshes { get; set; }

        [JsonPropertyName("last_failed_login")]
        public DateTime? LastFailedLogin { get; set; }
    }
}

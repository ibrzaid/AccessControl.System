using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsManagementResponse
    {
        [JsonPropertyName("total_grants")]
        public int TotalGrants { get; set; }

        [JsonPropertyName("total_creates")]
        public int TotalCreates { get; set; }

        [JsonPropertyName("total_deletes")]
        public int TotalDeletes { get; set; }

        [JsonPropertyName("total_updates")]
        public int TotalUpdates { get; set; }

        [JsonPropertyName("total_assignments")]
        public int TotalAssignments { get; set; }

        [JsonPropertyName("total_status_changes")]
        public int TotalStatusChanges { get; set; }

        [JsonPropertyName("total_password_resets")]
        public int TotalPasswordResets { get; set; }
    }
}

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.Timezone
{
    public class DisplaySettingsResponse : BaseResponse
    {
        [JsonPropertyName("user_timezone")]
        [Display(Name = "User Timezone")]
        public string? UserTimezone { get; set; }

        [JsonPropertyName("workspace_timezone")]
        [Display(Name = "Workspace Timezone")]
        public string WorkspaceTimezone { get; set; } = "UTC";

        [JsonPropertyName("effective_timezone")]
        [Display(Name = "Effective Timezone")]
        public string EffectiveTimezone { get; set; } = "UTC";

        [JsonPropertyName("workspace_date_format")]
        [Display(Name = "Date Format")]
        public string WorkspaceDateFormat { get; set; } = "YYYY-MM-DD";

        [JsonPropertyName("workspace_language")]
        [Display(Name = "Workspace Language")]
        public string WorkspaceLanguage { get; set; } = "en-US";

        [JsonPropertyName("user_language")]
        [Display(Name = "User Language")]
        public string? UserLanguage { get; set; }

        [JsonPropertyName("effective_language")]
        [Display(Name = "Effective Language")]
        public string EffectiveLanguage { get; set; } = "en-US";

        [JsonPropertyName("current_time_utc")]
        [Display(Name = "Current Time (UTC)")]
        public DateTime CurrentTimeUtc { get; set; }

        [JsonPropertyName("current_time_local")]
        [Display(Name = "Current Time (Local)")]
        public DateTime CurrentTimeLocal { get; set; }
    }
}

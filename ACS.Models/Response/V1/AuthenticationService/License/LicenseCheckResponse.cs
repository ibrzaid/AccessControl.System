using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.License
{
    public class LicenseCheckResponse : BaseResponse
    {
        [JsonPropertyName("is_valid")]
        [Display(Name = "Valid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("errors")]
        [Display(Name = "Errors")]
        public List<string> Errors { get; set; } = [];

        [JsonPropertyName("warnings")]
        [Display(Name = "Warnings")]
        public List<string> Warnings { get; set; } = [];

        [JsonPropertyName("current_users")]
        [Display(Name = "Current Users")]
        public int CurrentUsers { get; set; }

        [JsonPropertyName("max_users")]
        [Display(Name = "Max Users")]
        public int MaxUsers { get; set; }

        [JsonPropertyName("remaining_slots")]
        [Display(Name = "Remaining Slots")]
        public int RemainingSlots { get; set; }

        [JsonPropertyName("workspace_timezone")]
        [Display(Name = "Workspace Timezone")]
        public string WorkspaceTimezone { get; set; } = "UTC";

        [JsonPropertyName("workspace_date_format")]
        [Display(Name = "Date Format")]
        public string WorkspaceDateFormat { get; set; } = "YYYY-MM-DD";
    }
}

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.License
{
    public class SeatSummaryResponse : BaseResponse
    {
        [JsonPropertyName("max_users")]
        [Display(Name = "Max Users")]
        public int MaxUsers { get; set; }

        [JsonPropertyName("current_users")]
        [Display(Name = "Current Users")]
        public int CurrentUsers { get; set; }

        [JsonPropertyName("remaining_slots")]
        [Display(Name = "Remaining Slots")]
        public int RemainingSlots { get; set; }

        [JsonPropertyName("usage_pct")]
        [Display(Name = "Usage %")]
        public decimal UsagePct { get; set; }

        [JsonPropertyName("license_status")]
        [Display(Name = "License Status")]
        public string LicenseStatus { get; set; } = string.Empty;

        [JsonPropertyName("contract_end")]
        [Display(Name = "Contract End")]
        public string ContractEnd { get; set; } = string.Empty;

        [JsonPropertyName("days_to_expiry")]
        [Display(Name = "Days to Expiry")]
        public int DaysToExpiry { get; set; }

        [JsonPropertyName("by_status")]
        [Display(Name = "By Status")]
        public Dictionary<string, int> ByStatus { get; set; } = [];
    }
}

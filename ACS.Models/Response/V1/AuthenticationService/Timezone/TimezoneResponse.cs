using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.Timezone
{
    public class TimezoneResponse
    {
        [JsonPropertyName("name")]
        [Display(Name = "Timezone")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("abbrev")]
        [Display(Name = "Abbreviation")]
        public string Abbrev { get; set; } = string.Empty;

        [JsonPropertyName("utc_offset")]
        [Display(Name = "UTC Offset (hours)")]
        public double UtcOffset { get; set; }

        [JsonPropertyName("utc_offset_label")]
        [Display(Name = "UTC Offset")]
        public string UtcOffsetLabel { get; set; } = string.Empty;

        [JsonPropertyName("is_dst")]
        [Display(Name = "DST")]
        public bool IsDst { get; set; }

        [JsonPropertyName("region")]
        [Display(Name = "Region")]
        public string Region { get; set; } = string.Empty;
    }
}

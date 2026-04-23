using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.Timezone
{
    public class GetTimezonesResponse: BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "Timezones")]
        public List<TimezoneResponse> Data { get; set; } = [];
    }
}

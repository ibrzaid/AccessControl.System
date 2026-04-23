using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsResponse : BaseResponse
    {
        [JsonPropertyName("total")]
        public long Total { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("data")]
        public List<UserLogsDatumResponse>? Data { get; set; }


        [JsonPropertyName("analytics")]
        public UserLogsAnalyticsResponse? Analytics { get; set; }
    }
}

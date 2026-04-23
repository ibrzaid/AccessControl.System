using ACS.Models.Response;
using System.Text.Json.Serialization;

namespace ACS.Models.Request.V1.ParkingService.Session
{
    public class ParkingStatsResponse: BaseResponse
    {
        [JsonPropertyName("data")]
        public Dictionary<string, object>? Data { get; set; }
    }
}

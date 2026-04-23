
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.ParkingService.Entry
{
    public class EntrySessionResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        public ParkingSessionDataResponse? Data { get; set; }

        [JsonPropertyName("metadata")]
        public MetadataResponse? Metadata { get; set; }

        [JsonPropertyName("system_error")]
        public string? SystemError { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

       
    }
}

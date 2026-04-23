

using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.Hardware
{
    public class GetHardwareResponse : BaseResponse
    {
        [JsonPropertyName("total")] public int Total { get; set; }
        [JsonPropertyName("can_create")] public bool CanCreate { get; set; }
        [JsonPropertyName("data")] public List<HardwareItemResponse> Data { get; set; } = [];
    }
}

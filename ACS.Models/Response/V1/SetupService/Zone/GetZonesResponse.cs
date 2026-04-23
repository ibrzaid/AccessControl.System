

using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.Zone
{
    public class GetZonesResponse : BaseResponse
    {
        [JsonPropertyName("total")] public int Total { get; set; }
        [JsonPropertyName("can_create")] public bool CanCreate { get; set; }
        [JsonPropertyName("data")] public List<ZoneItemResponse> Data { get; set; } = [];
    }
}

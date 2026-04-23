
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.AccessPoint
{
    public class GetAccessPointsResponse : BaseResponse
    {
        [JsonPropertyName("total")] public int Total { get; set; }
        [JsonPropertyName("can_create")] public bool CanCreate { get; set; }
        [JsonPropertyName("data")] public List<AccessPointItemResponse> Data { get; set; } = [];
    }
}

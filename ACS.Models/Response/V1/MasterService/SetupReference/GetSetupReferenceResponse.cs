
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.MasterService.SetupReference
{
    public class GetSetupReferenceResponse : BaseResponse
    {
        [JsonPropertyName("data")] public List<SetupReferenceItemResponse> Data { get; set; } = [];
    }
}

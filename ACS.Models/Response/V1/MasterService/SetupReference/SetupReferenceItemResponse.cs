
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.MasterService.SetupReference
{
    public class SetupReferenceItemResponse
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("code")] public string? Code { get; set; }
        [JsonPropertyName("names")] public Dictionary<string, string>? Names { get; set; }
        [JsonPropertyName("config")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public Dictionary<string, FieldConfigResponse>? Config { get; set; }
    }
}

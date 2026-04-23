
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.MasterService.SetupReference
{
    public class FieldConfigResponse
    {
        [JsonPropertyName("exp")] public string? Exp { get; set; }
        [JsonPropertyName("text")] public Dictionary<string, string>? Text { get; set; }
        [JsonPropertyName("required")] public bool Required { get; set; }
        [JsonPropertyName("field_name")] public string? FieldName { get; set; }
    }
}


using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.Hardware
{
    public class OperationHardwareResultResponse : BaseResponse
    {
        [JsonPropertyName("project_id")] public int? ProjectId { get; set; }
        [JsonPropertyName("project_area_id")] public int? ProjectAreaId { get; set; }
        [JsonPropertyName("zone_id")] public int? ZoneId { get; set; }
        [JsonPropertyName("access_point_id")] public int? AccessPointId { get; set; }
        [JsonPropertyName("hardware_id")] public int? HardwareId { get; set; }
    }
}

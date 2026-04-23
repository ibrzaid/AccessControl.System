
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.Project
{
    public class GetProjectsResponse : BaseResponse
    {
        [JsonPropertyName("total")] public int Total { get; set; }
        [JsonPropertyName("can_create_project")] public bool CanCreateProject { get; set; }
        [JsonPropertyName("data")] public List<ProjectItemResponse> Data { get; set; } = [];
    }
}

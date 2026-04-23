
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.HierarchyAccess
{
    public class HierarchyProjectResponse
    {
        [JsonPropertyName("project_id")]
        [Display(Name = "Project ID")]
        public int ProjectId { get; set; }

        [JsonPropertyName("project_names")]
        [Display(Name = "Project Name")]
        public Dictionary<string, string>? ProjectNames { get; set; }

        [JsonPropertyName("project_description")]
        [Display(Name = "Description")]
        public string? ProjectDescription { get; set; }

        [JsonPropertyName("project_latitude")]
        [Display(Name = "Latitude")]
        public decimal? ProjectLatitude { get; set; }

        [JsonPropertyName("project_longitude")]
        [Display(Name = "Longitude")]
        public decimal? ProjectLongitude { get; set; }

        [JsonPropertyName("project_areas")]
        [Display(Name = "Project Areas")]
        public List<HierarchyProjectAreaResponse> ProjectAreas { get; set; } = [];
    }
}



using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACS.Models.Request.V1.SetupService.Project
{
    public class CreateProjectRequest : BaseRequest
    {
        [JsonPropertyName("project_names")]
        [Required(ErrorMessage = "Project names are required")]
        public Dictionary<string, string> ProjectNames { get; set; } = [];

        [JsonPropertyName("description")]
        public string? Description { get; set; }


        [JsonPropertyName("project_type")]
        [Required(ErrorMessage = "Project Type are required")]
        public int ProjectType { get; set; }

        [JsonPropertyName("address")]
        [StringLength(500)]
        public string? Address { get; set; }

        [JsonPropertyName("city")]
        [StringLength(100)]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        [StringLength(100)]
        public string? State { get; set; }

        [JsonPropertyName("country_id")]
        public int? CountryId { get; set; }

        [JsonPropertyName("postal_code")]
        [StringLength(20)]
        public string? PostalCode { get; set; }

        [JsonPropertyName("project_latitude")]
        public decimal? ProjectLatitude { get; set; }

        [JsonPropertyName("project_longitude")]
        public decimal? ProjectLongitude { get; set; }

        [JsonPropertyName("timezone")]
        [StringLength(50)]
        public string? Timezone { get; set; }

        [JsonPropertyName("is_public")]
        public bool IsPublic { get; set; } = false;
    }
}

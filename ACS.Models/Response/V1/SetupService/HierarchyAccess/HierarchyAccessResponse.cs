using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.SetupService.HierarchyAccess
{
    public class HierarchyAccessResponse : BaseResponse
    {
        /// <summary>
        /// Tells the frontend what the top-level items in data[] are:
        ///   "project"      → WORKSPACE scope
        ///   "project_area" → PROJECT scope
        ///   "zone"         → PROJECT_AREA scope
        ///   "access_point" → ZONE scope
        ///   "hardware"     → ACCESS_POINT scope
        /// </summary>
        [JsonPropertyName("result_level")]
        [Display(Name = "Result Level")]
        public string ResultLevel { get; set; } = string.Empty;

        [JsonPropertyName("total")]
        [Display(Name = "Total")]
        public int Total { get; set; }

        [JsonPropertyName("id")]
        [Display(Name = "ID")]
        public int ID { get; set; }


        [JsonPropertyName("names")]
        [Display(Name = "names")]
        public Dictionary<string, string>? Names { get; set; }

        /// <summary>
        /// Raw data array. The frontend should inspect result_level
        /// to determine how to parse each item.
        /// </summary>
        [JsonPropertyName("data")]
        [Display(Name = "Data")]
        public object Data { get; set; } = new List<object>();
    }
}

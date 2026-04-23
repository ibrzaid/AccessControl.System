using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.SetupService.HierarchyAccess
{
    public class HierarchyAccessPointResponse
    {
        [JsonPropertyName("access_point_id")]
        [Display(Name = "Access Point ID")]
        public int AccessPointId { get; set; }

        [JsonPropertyName("access_point_name")]
        [Display(Name = "Name")]
        public Dictionary<string, string>? AccessPointName { get; set; }

        [JsonPropertyName("prefix")]
        [Display(Name = "Prefix")]
        public string? Prefix { get; set; }

        [JsonPropertyName("serial_number")]
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }

        [JsonPropertyName("access_point_type_code")]
        [Display(Name = "Type")]
        public string? AccessPointTypeCode { get; set; }

        [JsonPropertyName("access_point_type_names")]
        [Display(Name = "Type Name")]
        public Dictionary<string, string>? AccessPointTypeNames { get; set; }

        [JsonPropertyName("access_point_latitude")]
        [Display(Name = "Latitude")]
        public decimal? AccessPointLatitude { get; set; }

        [JsonPropertyName("access_point_longitude")]
        [Display(Name = "Longitude")]
        public decimal? AccessPointLongitude { get; set; }

        [JsonPropertyName("zone_id")]
        [Display(Name = "Zone")]
        public long? ZoneId { get; set; }

        [JsonPropertyName("is_active")]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("hardware")]
        [Display(Name = "Hardware")]
        public List<HierarchyHardwareResponse> Hardware { get; set; } = [];
    }
}

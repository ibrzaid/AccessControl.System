using System.ComponentModel;
using ACS.Models.Attribute.V1;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.ANPRService
{
    public class AnprInsertRequest : BaseRequest
    {
        [JsonPropertyName("vehicle_image")]
        [Display(Name = "Vehicle Images")]
        [Required(ErrorMessage = "Vehicle Image is required")]
        public IFormFile? VehicleImage { get; init; }

        [JsonPropertyName("plate_image")]
        [Display(Name = "Plate Crop")]
        public IFormFile? PlateCrop { get; init; }

        [DefaultValue(150)]
        [JsonPropertyName("plate_country_id")]
        [Display(Name = "Country Id")]
        [Required(ErrorMessage = "Country Id is required")]
        [Range(1, int.MaxValue)]
        public int CountryId { get; init; }

        [DefaultValue(1)]
        [JsonPropertyName("plate_state_id")]
        [Display(Name = "Plate State Id")]
        [Required(ErrorMessage = "Plate State Id is required")]
        [Range(1, int.MaxValue)]
        public int PlateStateId { get; init; }

        [DefaultValue(1)]
        [JsonPropertyName("plate_category_id")]
        [Display(Name = "Plate Category Id")]
        [Required(ErrorMessage = "Plate Category Id is required")]
        [Range(1, int.MaxValue)]
        public int PlateCategoryId { get; init; }

        [DefaultValue("KSA")]
        [JsonPropertyName("plate_code")]
        [Display(Name = "Plate Code")]
        [StringLength(10)]
        public string? PlateCode { get; init; }

        [DefaultValue("1234")]
        [JsonPropertyName("plate_number")]
        [Display(Name = "Plate Number")]
        [Required(ErrorMessage = "Plate Number is required")]
        [StringLength(20, MinimumLength = 1)]
        public string? PlateNumber { get; init; }

        [DefaultValue(50)]
        [JsonPropertyName("confidence_score")]
        [Display(Name = "Confidence Score")]
        [Required(ErrorMessage = "Confidence Score is required")]
        [Range(0, 100)]
        public double ConfidenceScore { get; init; }

        [JsonPropertyName("detection_time")]
        [Display(Name = "Detection Time")]
        [Required(ErrorMessage = "Detection Time is required")]
        public DateTime DetectionTime { get; init; }

        [JsonPropertyName("vehicle_color")]
        [Display(Name = "Vehicle Color")]
        [StringLength(50)]
        public string? VehicleColor { get; init; }

        [JsonPropertyName("vehicle_make")]
        [Display(Name = "Vehicle Make")]
        [StringLength(50)]
        public string? VehicleMake { get; init; }

        [JsonPropertyName("vehicle_model")]
        [Display(Name = "Vehicle Mode")]
        [StringLength(50)]
        public string? VehicleModel { get; init; }

        [JsonPropertyName("vehicle_type")]
        [Display(Name = "Vehicle Type")]
        [StringLength(50)]
        public string? VehicleType { get; init; }

        [DefaultValue(80)]
        [JsonPropertyName("vehicle_speed_kmh")]
        [Display(Name = "Vehicle Speed Kmh")]
        [Required(ErrorMessage = "Vehicle Speed Kmh is required")]
        [Range(0, 500)]
        public double VehicleSpeedKmh { get; init; }

        [DefaultValue(20)]
        [JsonPropertyName("processing_time_ms")]
        [Display(Name = "Processing Time Ms")]
        [Required(ErrorMessage = "Processing Time Ms is required")]
        [Range(0, 100000)]
        public int ProcessingTimeMs { get; init; }

        [JsonPropertyName("raw_data")]
        [Display(Name = "Row Date")]
        public string? RawData { get; init; }

        [DefaultValue("0")]
        [JsonPropertyName("capture_latitude")]
        [Display(Name = "Capture Latitude")]
        [Required(ErrorMessage = "Capture Latitude is required")]
        [LatLong]
        [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Capture Latitude must be a valid number")]
        public string? CaptureLatitude { get; init; }

        [DefaultValue("0")]
        [JsonPropertyName("capture_longitude")]
        [Display(Name = "Capture Longitude")]
        [Required(ErrorMessage = "Capture Longitude is required")]
        [LatLong]
        [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Capture Longitude must be a valid number")]
        public string? CaptureLongitude { get; init; }

        [DefaultValue("engine.0.1.1.7")]
        [JsonPropertyName("anpr_engine")]
        [Display(Name = "ANPR Engin")]
        [Required(ErrorMessage = "ANPR Engine is required")]
        public string? AnprEngine { get; init; }

        [DefaultValue("UNKNOWN")]
        [JsonPropertyName("direction")]
        [Display(Name = "Direction")]
        [Required(ErrorMessage = "Direction is required")]
        [RegularExpression("^(ENTER|EXIT|UNKNOWN)$", ErrorMessage = "Direction must be ENTER, EXIT, or UNKNOWN")]
        public string? Direction { get; init; }
    }

}

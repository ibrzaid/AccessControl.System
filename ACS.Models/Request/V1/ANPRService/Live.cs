
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ACS.Models.Request.V1.ANPRService.Live
{
    public record LiveDataRequest(
        string? Latitude,
        string? Longitude,

        [property: JsonPropertyName("view_type")]
        [property: RegularExpression(
            @"^(auto|projects|areas|zones|zone_children|access_point|hardware|live|heatmap|live_heatmap|clusters|tracking|3d|satellite)$",
            ErrorMessage = "Invalid view type")]
        string ViewType = "auto",

        [property: JsonPropertyName("project_id")] int? ProjectId = null,
        [property: JsonPropertyName("project_area_id")] int? ProjectAreaId = null,
        [property: JsonPropertyName("zone_id")] long? ZoneId = null,
        [property: JsonPropertyName("access_point_id")] int? AccessPointId = null,
        [property: JsonPropertyName("hardware_id")] int? HardwareId = null,
        [property: JsonPropertyName("bounds")] BoundsRequest? Bounds = null,

        [property: JsonPropertyName("center_lat")]
        [property: Range(-90, 90)]   double? CenterLat = null,

        [property: JsonPropertyName("center_lng")]
        [property: Range(-180, 180)] double? CenterLng = null,

        [property: JsonPropertyName("zoom_level")]
        [property: Range(0, 22)]     int ZoomLevel = 12,

        [property: JsonPropertyName("from_time")] DateTime? FromTime = null,
        [property: JsonPropertyName("to_time")] DateTime? ToTime = null,

        [property: JsonPropertyName("window_minutes")]
        [property: Range(1, 1440)]   int WindowMinutes = 5,

        [property: JsonPropertyName("min_confidence")]
        [property: Range(0, 100)]    double MinConfidence = 0,

        [property: JsonPropertyName("resolution")]
        [property: Range(10, 200)]   int Resolution = 50,

        [property: JsonPropertyName("cluster_radius")]
        [property: Range(10, 500)]   int ClusterRadius = 50,

        [property: JsonPropertyName("weight_by_confidence")] bool WeightByConfidence = true,
        [property: JsonPropertyName("include_trend")] bool IncludeTrend = true,

        [property: JsonPropertyName("plate_code")]
        [property: StringLength(10)]  string? PlateCode = null,

        [property: JsonPropertyName("plate_number")]
        [property: StringLength(20)]  string? PlateNumber = null,

        [property: JsonPropertyName("include_path")] bool IncludePath = true,
        [property: JsonPropertyName("include_stops")] bool IncludeStops = true,

        [property: JsonPropertyName("height_factor")]
        [property: Range(0.1, 10.0)] double HeightFactor = 1.0,

        [property: JsonPropertyName("limit")]
        [property: Range(1, 10000)]  int Limit = 500,

        [property: JsonPropertyName("offset")]
        [property: Range(0, int.MaxValue)] int Offset = 0) : BaseRequests(Latitude, Longitude)
    {
        public bool IsValidForTracking() =>
            !string.IsNullOrEmpty(PlateCode) && !string.IsNullOrEmpty(PlateNumber);

        public LiveDataRequest WithDefaultTimes() =>
            this with
            {
                FromTime = FromTime ?? DateTime.UtcNow.AddDays(-1),
                ToTime   = ToTime   ?? DateTime.UtcNow
            };
    }


    public record BoundsRequest(
        [property: Required]
        [property: Range(-90, 90)]
        [property: JsonPropertyName("south")] double South,

        [property: Required]
        [property: Range(-90, 90)]
        [property: JsonPropertyName("north")] double North,

        [property: Required]
        [property: Range(-180, 180)]
        [property: JsonPropertyName("west")]  double West,

        [property: Required]
        [property: Range(-180, 180)]
        [property: JsonPropertyName("east")]  double East,

        [property: JsonPropertyName("center_lat")] double? CenterLat = null,
        [property: JsonPropertyName("center_lng")] double? CenterLng = null
    )
    {
        public bool IsValid() => South <= North && West <= East;
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure

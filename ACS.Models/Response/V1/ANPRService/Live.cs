using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable IDE0130s
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ACS.Models.Response.V1.ANPRService.Live
{
    // =========================================================================
    // ROOT RESPONSE
    // =========================================================================
    public record LiveResponse(
        bool Success,
        string Message,
        string ErrorCode,
        string RequestId,

        // ── Always present on success
        [property: JsonPropertyName("current_level")] string? CurrentLevel,
        [property: JsonPropertyName("available_views")] string[]? AvailableViews,
        [property: JsonPropertyName("breadcrumb")] List<MapBreadcrumbItem>? Breadcrumb,
        [property: JsonPropertyName("hierarchy_ids")] MapHierarchyIdsResponse? HierarchyIds,
        [property: JsonPropertyName("master")] MapMasterResponse? Master,

        // ── FeatureCollection (absent in hardware level)
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("features")] List<JsonElement>? Features,
        [property: JsonPropertyName("total")] long? Total,    // absent at access_point level

        // ── Bounds
        [property: JsonPropertyName("bounds")] MapBoundsResponse? Bounds,

        // ── Heatmap / live_heatmap
        [property: JsonPropertyName("data")] List<MapHeatmapCellResponse>? Data,
        [property: JsonPropertyName("hotspots")] List<MapHotspotResponse>? Hotspots,
        [property: JsonPropertyName("live_config")] MapLiveConfigResponse? LiveConfig,

        // ── Stats (raw JsonElement — shape per view type documented below)
        [property: JsonPropertyName("stats")] JsonElement? Stats,

        // ── Pagination + timing (live only)
        [property: JsonPropertyName("pagination")] MapPaginationResponse? Pagination,
        [property: JsonPropertyName("timestamp")] DateTime? Timestamp,
        [property: JsonPropertyName("refresh_interval")] int? RefreshInterval,

        // ── Level-detail objects
        [property: JsonPropertyName("zone_info")] MapZoneInfoResponse? ZoneInfo,         // zone_children
        [property: JsonPropertyName("access_point")] MapAccessPointInfoResponse? AccessPoint,      // access_point level
        [property: JsonPropertyName("total_hardware")] int? TotalHardware,    // access_point level
        [property: JsonPropertyName("hardware")] MapHardwareInfoResponse? Hardware,         // hardware level
        [property: JsonPropertyName("recent_detections")] List<JsonElement>? RecentDetections, // hardware level

        // ── 3D camera
        [property: JsonPropertyName("camera")] MapCameraConfigResponse? Camera,

        // ── Satellite
        [property: JsonPropertyName("center")] MapCoordinateResponse? Center,
        [property: JsonPropertyName("radius")] int? Radius,

        // ── Shared
        [property: JsonPropertyName("filters")] JsonElement? Filters,
        [property: JsonPropertyName("metadata")] MapMetadataResponse? Metadata

    ) : BaseResponses(Success, Message, ErrorCode, RequestId);


    // =========================================================================
    // BREADCRUMB / HIERARCHY
    // =========================================================================
    public record MapBreadcrumbItem(
        [property: JsonPropertyName("id")] object Id,
        [property: JsonPropertyName("name")] JsonElement? Name,
        [property: JsonPropertyName("code")] string? Code,
        [property: JsonPropertyName("type")] string Type
    );

    public record MapHierarchyIdsResponse(
        [property: JsonPropertyName("workspace_id")] int WorkspaceId,
        [property: JsonPropertyName("project_id")] int? ProjectId,
        [property: JsonPropertyName("project_area_id")] int? ProjectAreaId,
        [property: JsonPropertyName("zone_id")] long? ZoneId,
        [property: JsonPropertyName("access_point_id")] int? AccessPointId,
        [property: JsonPropertyName("hardware_id")] int? HardwareId
    );


    // =========================================================================
    // MASTER / METADATA
    // =========================================================================
    public record MapMasterResponse(
        [property: JsonPropertyName("requested_view")] string RequestedView,
        [property: JsonPropertyName("effective_view")] string EffectiveView,
        [property: JsonPropertyName("function_called")] string FunctionCalled,
        [property: JsonPropertyName("processing_time_ms")] double ProcessingTimeMs,
        [property: JsonPropertyName("audit_log_id")] int? AuditLogId,
        [property: JsonPropertyName("timestamp")] DateTime Timestamp
    );

    public record MapMetadataResponse(
        [property: JsonPropertyName("processing_time_ms")] double ProcessingTimeMs,
        [property: JsonPropertyName("timestamp")] DateTime Timestamp
    );


    // =========================================================================
    // BOUNDS / CAMERA / COORDINATE
    // =========================================================================

    /// south/north/west/east present at projects/areas/zones/heatmap levels.
    /// zoom only at zone_children(17), access_point(18), hardware(19).
    /// All use flat center_lat/center_lng — NOT a nested object.
    public record MapBoundsResponse(
        [property: JsonPropertyName("south")] double? South,
        [property: JsonPropertyName("north")] double? North,
        [property: JsonPropertyName("west")] double? West,
        [property: JsonPropertyName("east")] double? East,
        [property: JsonPropertyName("center_lat")] double? CenterLat,
        [property: JsonPropertyName("center_lng")] double? CenterLng,
        [property: JsonPropertyName("zoom")] int? Zoom
    );

    /// 3D map camera (NOT a physical camera).
    /// DB: camera.position.{lat,lng,altitude,pitch,bearing}, max_pitch, min_pitch
    public record MapCameraConfigResponse(
        [property: JsonPropertyName("position")] MapCameraPositionResponse Position,
        [property: JsonPropertyName("max_pitch")] int MaxPitch,
        [property: JsonPropertyName("min_pitch")] int MinPitch
    );

    public record MapCameraPositionResponse(
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("lng")] double Lng,
        [property: JsonPropertyName("altitude")] double Altitude,
        [property: JsonPropertyName("pitch")] int Pitch,
        [property: JsonPropertyName("bearing")] int Bearing
    );

    /// Generic {lat,lng} coordinate used by satellite.center and AP coordinates.
    public record MapCoordinateResponse(
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("lng")] double Lng
    );


    // =========================================================================
    // LEVEL-DETAIL OBJECTS
    // =========================================================================

    /// zone_children — parent zone summary.
    /// DB: zone_names->>'en-US' → single string (NOT a dict)
    public record MapZoneInfoResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("name")] JsonElement? Name,   // zone_names->>'en-US'
        [property: JsonPropertyName("code")] string? Code,
        [property: JsonPropertyName("total_spots")] int TotalSpots,
        [property: JsonPropertyName("available_spots")] int AvailableSpots,
        [property: JsonPropertyName("center_lat")] double CenterLat,
        [property: JsonPropertyName("center_lng")] double CenterLng
    );

    /// access_point level — AP detail.
    /// DB: names = ap.access_point_name (jsonb dict), type_names = apt.access_point_type_names (jsonb dict)
    /// DB: coordinates = {lat, lng}
    public record MapAccessPointInfoResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("names")] JsonElement? Names,
        [property: JsonPropertyName("prefix")] string? Prefix,
        [property: JsonPropertyName("type_code")] string? TypeCode,
        [property: JsonPropertyName("type_names")] JsonElement? TypeNames,
        [property: JsonPropertyName("coordinates")] MapCoordinateResponse? Coordinates,
        [property: JsonPropertyName("orientation")] int? Orientation,
        [property: JsonPropertyName("position_x")] double? PositionX,
        [property: JsonPropertyName("position_y")] double? PositionY,
        [property: JsonPropertyName("is_active")] bool IsActive,
        [property: JsonPropertyName("hardware_count")] int HardwareCount,
        [property: JsonPropertyName("detections_24h")] long? Detections24h
    );

    /// hardware level — full hardware detail.
    /// type_names/status_names = jsonb dicts.  configuration = arbitrary jsonb.
    /// location = {lat,lng,access_points(dict),zone(dict),area(dict),project(dict)}
    /// performance_24h = {total, avg_confidence, peak_hour}
    public record MapHardwareInfoResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("type_names")] JsonElement? TypeNames,
        [property: JsonPropertyName("manufacturer")] string? Manufacturer,
        [property: JsonPropertyName("model")] string? Model,
        [property: JsonPropertyName("serial_number")] string? SerialNumber,
        [property: JsonPropertyName("firmware_version")] string? FirmwareVersion,
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("status_names")] JsonElement? StatusNames,
        [property: JsonPropertyName("last_maintenance")] DateTime? LastMaintenance,
        [property: JsonPropertyName("next_maintenance")] DateTime? NextMaintenance,
        [property: JsonPropertyName("configuration")] JsonElement? Configuration,
        [property: JsonPropertyName("last_activity")] DateTime? LastActivity,
        [property: JsonPropertyName("detection_count")] int DetectionCount,
        [property: JsonPropertyName("is_active")] bool IsActive,
        [property: JsonPropertyName("created_at")] DateTime? CreatedAt,
        [property: JsonPropertyName("location")] MapHardwareLocationResponse? Location,
        [property: JsonPropertyName("performance_24h")] MapHardwarePerformanceResponse? Performance24h
    );

    public record MapHardwareLocationResponse(
        [property: JsonPropertyName("lat")] double? Lat,
        [property: JsonPropertyName("lng")] double? Lng,
        [property: JsonPropertyName("access_points")] JsonElement? AccessPoints, // access_point_name jsonb dict
        [property: JsonPropertyName("zone")] JsonElement? Zone,          // zone_names jsonb dict
        [property: JsonPropertyName("area")] JsonElement? Area,          // project_area_names jsonb dict
        [property: JsonPropertyName("project")] JsonElement? Project        // project_names jsonb dict
    );

    public record MapHardwarePerformanceResponse(
        [property: JsonPropertyName("total")] long? Total,
        [property: JsonPropertyName("avg_confidence")] double? AvgConfidence,
        [property: JsonPropertyName("peak_hour")] double? PeakHour       // MODE() returns numeric
    );


    // =========================================================================
    // HEATMAP CELLS / HOTSPOTS / LIVE CONFIG
    // =========================================================================
    public record MapHeatmapCellResponse(
        [property: JsonPropertyName("x")] int X,
        [property: JsonPropertyName("y")] int Y,
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("lng")] double Lng,
        [property: JsonPropertyName("count")] int Count,
        [property: JsonPropertyName("weight")] double Weight,
        [property: JsonPropertyName("intensity")] double Intensity,
        [property: JsonPropertyName("avg_confidence")] double? AvgConfidence,
        [property: JsonPropertyName("unique_plates")] int? UniquePlates,
        [property: JsonPropertyName("last_detection")] DateTime? LastDetection,
        [property: JsonPropertyName("is_hotspot")] bool? IsHotspot,       // live_heatmap only
        [property: JsonPropertyName("dominant_color")] string? DominantColor    // live_heatmap only
    );

    public record MapHotspotResponse(
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("lng")] double Lng,
        [property: JsonPropertyName("intensity")] double Intensity,
        [property: JsonPropertyName("rank")] int Rank
    );

    public record MapLiveConfigResponse(
        [property: JsonPropertyName("window_minutes")] int WindowMinutes,
        [property: JsonPropertyName("refresh_interval_seconds")] int RefreshIntervalSeconds,
        [property: JsonPropertyName("next_update")] DateTime NextUpdate,
        [property: JsonPropertyName("resolution")] int Resolution,
        [property: JsonPropertyName("total_cells")] int TotalCells
    );


    // =========================================================================
    // PAGINATION
    // =========================================================================
    public record MapPaginationResponse(
        [property: JsonPropertyName("total")] int Total,
        [property: JsonPropertyName("limit")] int Limit,
        [property: JsonPropertyName("offset")] int Offset,
        [property: JsonPropertyName("has_more")] bool HasMore
    );


    // =========================================================================
    // STATS — one typed record per view type (for callers who want to deserialise Stats)
    // =========================================================================

    /// live — verified from fun_get_live_detections
    public record MapLiveStatsResponse(
        [property: JsonPropertyName("total")] long Total,
        [property: JsonPropertyName("unique_plates")] long UniquePlates,
        [property: JsonPropertyName("avg_confidence")] double? AvgConfidence,
        [property: JsonPropertyName("oldest")] DateTime? Oldest,
        [property: JsonPropertyName("newest")] DateTime? Newest,
        [property: JsonPropertyName("by_access_point")] Dictionary<string, int>? ByAccessPoint,
        [property: JsonPropertyName("by_vehicle_type")] Dictionary<string, int>? ByVehicleType,
        [property: JsonPropertyName("by_confidence_range")] MapConfidenceRangeResponse? ByConfidenceRange,
        [property: JsonPropertyName("blacklisted_count")] int? BlacklistedCount
    );

    public record MapConfidenceRangeResponse(
        [property: JsonPropertyName("high")] int High,
        [property: JsonPropertyName("medium")] int Medium,
        [property: JsonPropertyName("low")] int Low
    );

    /// heatmap — verified from fun_get_heatmap_data
    public record MapHeatmapStatsResponse(
        [property: JsonPropertyName("total_detections")] int TotalDetections,
        [property: JsonPropertyName("grid_cells_with_data")] int GridCellsWithData,
        [property: JsonPropertyName("max_intensity")] double MaxIntensity,
        [property: JsonPropertyName("avg_detections_per_cell")] double AvgDetectionsPerCell,
        [property: JsonPropertyName("time_range")] MapTimeRangeResponse? TimeRange,
        [property: JsonPropertyName("bounds")] JsonElement? Bounds,         // pin_bounds passthrough
        [property: JsonPropertyName("bounds_source")] string? BoundsSource,  // "provided"|"auto_detected"
        [property: JsonPropertyName("resolution")] int Resolution,
        [property: JsonPropertyName("total_cells")] int TotalCells
    );

    /// live_heatmap — verified from fun_get_live_heatmap_data
    public record MapLiveHeatmapStatsResponse(
        [property: JsonPropertyName("total_detections")] int TotalDetections,
        [property: JsonPropertyName("detection_rate_per_minute")] double DetectionRatePerMinute,
        [property: JsonPropertyName("grid_cells_with_data")] int GridCellsWithData,
        [property: JsonPropertyName("max_intensity")] double MaxIntensity,
        [property: JsonPropertyName("avg_detections_per_cell")] double AvgDetectionsPerCell,
        [property: JsonPropertyName("unique_plates_total")] long UniquePlatesTotal,
        [property: JsonPropertyName("time_window")] MapTimeWindowResponse? TimeWindow,
        [property: JsonPropertyName("trend")] MapTrendResponse? Trend,
        [property: JsonPropertyName("bounds_source")] string? BoundsSource
    );

    public record MapTimeWindowResponse(
        [property: JsonPropertyName("minutes")] int Minutes,
        [property: JsonPropertyName("from")] DateTime From,
        [property: JsonPropertyName("to")] DateTime To
    );

    public record MapTrendResponse(
        [property: JsonPropertyName("percentage")] double Percentage,
        [property: JsonPropertyName("direction")] string Direction,       // STABLE | UP | DOWN
        [property: JsonPropertyName("previous_count")] int PreviousCount,
        [property: JsonPropertyName("current_count")] int CurrentCount
    );

    /// clusters — verified from fun_get_detection_clusters
    public record MapClusterStatsResponse(
        [property: JsonPropertyName("total_detections")] int TotalDetections,
        [property: JsonPropertyName("total_clusters")] int TotalClusters,
        [property: JsonPropertyName("avg_detections_per_cluster")] double AvgDetectionsPerCluster,
        [property: JsonPropertyName("cluster_distance_degrees")] double ClusterDistanceDegrees,
        [property: JsonPropertyName("cluster_distance_meters")] double ClusterDistanceMeters,
        [property: JsonPropertyName("zoom_level")] int ZoomLevel,
        [property: JsonPropertyName("bounds")] MapStatsBoundsResponse? Bounds,
        [property: JsonPropertyName("grid_size")] int GridSize
    );

    /// tracking — verified from fun_track_vehicle
    public record MapTrackingStatsResponse(
        [property: JsonPropertyName("plate")] string? Plate,
        [property: JsonPropertyName("first_seen")] DateTime? FirstSeen,
        [property: JsonPropertyName("last_seen")] DateTime? LastSeen,
        [property: JsonPropertyName("total_detections")] int TotalDetections,
        [property: JsonPropertyName("unique_access_points")] int UniqueAccessPoints,
        [property: JsonPropertyName("unique_zones")] int UniqueZones,
        [property: JsonPropertyName("avg_confidence")] double? AvgConfidence,
        [property: JsonPropertyName("total_minutes")] double TotalMinutes,
        [property: JsonPropertyName("total_hours")] double TotalHours,
        [property: JsonPropertyName("most_common_color")] string? MostCommonColor,
        [property: JsonPropertyName("most_common_type")] string? MostCommonType,
        [property: JsonPropertyName("blacklist")] JsonElement? Blacklist,  // {is_blacklisted,reason,severity,list_type}
        [property: JsonPropertyName("time_range")] MapTimeRangeResponse? TimeRange
    );

    public record MapTimeRangeResponse(
        [property: JsonPropertyName("from")] DateTime? From,
        [property: JsonPropertyName("to")] DateTime? To
    );

    /// 3d — verified from fun_get_3d_view_data
    public record MapThreeDStatsResponse(
        [property: JsonPropertyName("total_zones")] int TotalZones,
        [property: JsonPropertyName("total_buildings")] int TotalBuildings,
        [property: JsonPropertyName("max_height")] double MaxHeight,
        [property: JsonPropertyName("height_factor")] double HeightFactor,
        [property: JsonPropertyName("bounds")] MapThreeDStatsBoundsResponse? Bounds
    );

    /// 3d stats.bounds has a nested center {lat,lng} (unlike top-level bounds which are flat)
    public record MapThreeDStatsBoundsResponse(
        [property: JsonPropertyName("south")] double South,
        [property: JsonPropertyName("north")] double North,
        [property: JsonPropertyName("west")] double West,
        [property: JsonPropertyName("east")] double East,
        [property: JsonPropertyName("center")] MapCoordinateResponse Center  // nested {lat,lng}
    );

    /// satellite — verified from fun_get_satellite_view
    public record MapSatelliteStatsResponse(
        [property: JsonPropertyName("center")] MapCoordinateResponse? Center,
        [property: JsonPropertyName("radius_meters")] int RadiusMeters,
        [property: JsonPropertyName("bounds")] MapStatsBoundsResponse? Bounds,
        [property: JsonPropertyName("counts")] MapSatelliteCountsResponse? Counts,
        [property: JsonPropertyName("time_range")] MapTimeRangeResponse? TimeRange
    );

    public record MapSatelliteCountsResponse(
        [property: JsonPropertyName("detections")] int Detections,
        [property: JsonPropertyName("access_points")] int AccessPoints,
        [property: JsonPropertyName("zones")] int Zones
    );

    /// Flat bounds used inside stats (clusters + satellite) — no zoom, no center
    public record MapStatsBoundsResponse(
        [property: JsonPropertyName("south")] double South,
        [property: JsonPropertyName("north")] double North,
        [property: JsonPropertyName("west")] double West,
        [property: JsonPropertyName("east")] double East
    );
}
#pragma warning restore IDE0130
#pragma warning restore IDE0130 // Namespace does not match folder structure
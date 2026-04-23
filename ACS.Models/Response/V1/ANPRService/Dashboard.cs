using System.Text.Json.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ACS.Models.Response.V1.ANPRService.Dashboard
{
    public record AnprDashboardResponse(
     bool Success,
     string? Message,
     string ErrorCode,
     string RequestId,
    [property: JsonPropertyName("module")] string? Module,       // always "anpr"
    [property: JsonPropertyName("enabled")] bool Enabled,
    [property: JsonPropertyName("generated_at")] DateTime? GeneratedAt,
    [property: JsonPropertyName("detail")] string? Detail,
    [property: JsonPropertyName("anpr")] AnprStatsResponse? Anpr,         // null when disabled
    [property: JsonPropertyName("hourly_trend")] HourlyBucketResponse[]? HourlyTrend,  // [] when disabled
    [property: JsonPropertyName("projects_detections")] ProjectDetectionResponse[]? ProjectsDetections
 ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    public record AnprStatsResponse(
        [property: JsonPropertyName("detections_today")] long DetectionsToday,
        [property: JsonPropertyName("detections_yesterday")] long DetectionsYesterday,
        [property: JsonPropertyName("detections_delta_pct")] decimal? DetectionsDeltaPct,  // null when yesterday = 0
        [property: JsonPropertyName("unique_plates_today")] long UniquePlatesToday,
        [property: JsonPropertyName("blacklist_hits_today")] long BlacklistHitsToday,
        [property: JsonPropertyName("avg_confidence")] decimal AvgConfidence,
        [property: JsonPropertyName("high_confidence")] long HighConfidence,
        [property: JsonPropertyName("low_confidence")] long LowConfidence,
        [property: JsonPropertyName("peak_hour")] int? PeakHour,
        [property: JsonPropertyName("peak_hour_count")] int? PeakHourCount
    );

    /// <summary>
    /// One bucket per hour from the hourly detection summary.
    /// Hour is formatted as HH24:MI (e.g. "14:00") in the workspace timezone.
    /// </summary>
    public record HourlyBucketResponse(
        [property: JsonPropertyName("hour")] string Hour,
        [property: JsonPropertyName("detections")] long Detections,
        [property: JsonPropertyName("blacklist")] long Blacklist
    );

    /// <summary>
    /// Per-project detection count — used to enrich project cards.
    /// </summary>
    public record ProjectDetectionResponse(
        [property: JsonPropertyName("project_id")] long ProjectId,
        [property: JsonPropertyName("detections_today")] long DetectionsToday
    );







    public record AnprDashboardProjectResponse(
        bool Success,
        string? Message,
        string ErrorCode,
        string RequestId,
        [property: JsonPropertyName("timestamp")] DateTime? Timestamp,
        [property: JsonPropertyName("workspace_id")] long? WorkspaceId,
        [property: JsonPropertyName("project_id")] long? ProjectId,
        [property: JsonPropertyName("scope")] string? Scope,
        [property: JsonPropertyName("view_type")] string? ViewType,
        [property: JsonPropertyName("timezone")] string? Timezone,
        [property: JsonPropertyName("metadata")] AnprMetadataResponse? Metadata,
        // ── detection_tbls_sch_v1.fun_anpr_get_detection_stats ────────────────
        [property: JsonPropertyName("selected_period")] AnprSelectedPeriodResponse? SelectedPeriod,
        [property: JsonPropertyName("kpis")] AnprKpisResponse? Kpis,
        [property: JsonPropertyName("hourly_trend")] IReadOnlyList<AnprHourlyTrendResponse>? HourlyTrend,
        [property: JsonPropertyName("daily_trend")] IReadOnlyList<AnprDailyTrendResponse>? DailyTrend,
        [property: JsonPropertyName("comparison")] AnprComparisonResponse? Comparison,
        [property: JsonPropertyName("vehicle_distribution")] IReadOnlyList<AnprVehicleTypeResponse>? VehicleDistribution,
        // ── setup_tbls_sch_v1.fun_anpr_get_access_point_stats ─────────────────
        [property: JsonPropertyName("hotspots")] IReadOnlyList<AnprHotspotResponse>? Hotspots,
        // ── hardware_tbls_sch_v1.fun_anpr_get_camera_stats ────────────────────
        [property: JsonPropertyName("system")] AnprSystemResponse? System,
        // ── plate_tables_v1.fun_anpr_get_plate_stats ──────────────────────────
        [property: JsonPropertyName("plate_hits")] AnprPlateHitsResponse? PlateHits,
        // ── detection_tbls_sch_v1.fun_get_recent_detections ───────────────────
        [property: JsonPropertyName("recent_detections")] AnprRecentDetectionsWrapperResponse? RecentDetections
    ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    // ── KPIs ──────────────────────────────────────────────────────────────────

    public record AnprKpisResponse(
        [property: JsonPropertyName("detections")] AnprDetectionKpiResponse Detections,
        [property: JsonPropertyName("unique_plates")] AnprSingleCountResponse UniquePlates,
        [property: JsonPropertyName("blacklist")] AnprSingleCountResponse Blacklist,
        [property: JsonPropertyName("whitelist")] AnprSingleCountResponse Whitelist,
        [property: JsonPropertyName("confidence")] AnprConfidenceKpiResponse Confidence,
        [property: JsonPropertyName("peak_hour")] AnprPeakHourResponse PeakHour
    );

    public record AnprDetectionKpiResponse(
        [property: JsonPropertyName("today")] long? Today,
        [property: JsonPropertyName("yesterday")] long? Yesterday,
        [property: JsonPropertyName("this_week")] long? ThisWeek,
        [property: JsonPropertyName("this_month")] long? ThisMonth,
        [property: JsonPropertyName("change_pct")] decimal? ChangePct
    );

    public record AnprSingleCountResponse(
        [property: JsonPropertyName("today")] long? Today
    );

    public record AnprConfidenceKpiResponse(
        [property: JsonPropertyName("today_avg")] decimal? TodayAvg
    );

    public record AnprPeakHourResponse(
        [property: JsonPropertyName("hour")] int? Hour,
        [property: JsonPropertyName("count")] int? Count
    );

    // ── Hourly trend ──────────────────────────────────────────────────────────

    public record AnprHourlyTrendResponse(
        [property: JsonPropertyName("hour")] int Hour,
        [property: JsonPropertyName("detections")] long? Detections,
        [property: JsonPropertyName("unique_plates")] long? UniquePlates,
        [property: JsonPropertyName("blacklist")] long? Blacklist,
        [property: JsonPropertyName("avg_confidence")] decimal? AvgConfidence
    );

    // ── Daily trend ───────────────────────────────────────────────────────────

    public record AnprDailyTrendResponse(
        [property: JsonPropertyName("date")] DateOnly Date,
        [property: JsonPropertyName("day_name")] string? DayName,
        [property: JsonPropertyName("day_of_week")] int? DayOfWeek,
        [property: JsonPropertyName("day_of_month")] int? DayOfMonth,
        [property: JsonPropertyName("detections")] long? Detections,
        [property: JsonPropertyName("unique_plates")] long? UniquePlates,
        [property: JsonPropertyName("blacklist")] long? Blacklist,
        [property: JsonPropertyName("avg_confidence")] decimal? AvgConfidence
    );

    // ── Comparison ────────────────────────────────────────────────────────────

    public record AnprComparisonResponse(
        [property: JsonPropertyName("current")] long? Current,
        [property: JsonPropertyName("previous")] long? Previous,
        [property: JsonPropertyName("change_pct")] decimal? ChangePct
    );

    // ── Vehicle distribution ──────────────────────────────────────────────────

    public record AnprVehicleTypeResponse(
        [property: JsonPropertyName("vehicle_type")] string VehicleType,
        [property: JsonPropertyName("count")] long? Count,
        [property: JsonPropertyName("percentage")] decimal? Percentage
    );

    // ── Hotspots ──────────────────────────────────────────────────────────────

    public record AnprHotspotResponse(
        [property: JsonPropertyName("access_point_id")] long AccessPointId,
        [property: JsonPropertyName("access_point_name")] string? AccessPointName,
        [property: JsonPropertyName("prefix")] string? Prefix,
        [property: JsonPropertyName("latitude")] decimal? Latitude,
        [property: JsonPropertyName("longitude")] decimal? Longitude,
        [property: JsonPropertyName("is_active")] bool IsActive,
        [property: JsonPropertyName("detections")] long Detections,
        [property: JsonPropertyName("unique_plates")] long UniquePlates,
        [property: JsonPropertyName("blacklist_hits")] long BlacklistHits,
        [property: JsonPropertyName("avg_confidence")] decimal AvgConfidence,
        [property: JsonPropertyName("peak_hour")] int? PeakHour,
        [property: JsonPropertyName("change_pct")] decimal? ChangePct
    );

    // ── System / cameras ─────────────────────────────────────────────────────

    public record AnprSystemResponse(
        [property: JsonPropertyName("cameras")] AnprCameraCountsResponse? Cameras,
        [property: JsonPropertyName("camera_list")] IReadOnlyList<CameraListItemResponse>? CameraList
    );

    public record AnprCameraCountsResponse(
        [property: JsonPropertyName("total")] int Total,
        [property: JsonPropertyName("online")] int Online,
        [property: JsonPropertyName("offline")] int Offline,
        [property: JsonPropertyName("warning")] int Warning,
        [property: JsonPropertyName("error")] int Error,
        [property: JsonPropertyName("online_pct")] decimal OnlinePct,
        [property: JsonPropertyName("uptime_pct")] decimal UptimePct
    );

    public record CameraListItemResponse(
        [property: JsonPropertyName("hardware_id")] long HardwareId,
        [property: JsonPropertyName("hardware_label")] string? HardwareLabel,
        [property: JsonPropertyName("serial_number")] string? SerialNumber,
        [property: JsonPropertyName("access_point_id")] long AccessPointId,
        [property: JsonPropertyName("access_point_name")] string? AccessPointName,
        [property: JsonPropertyName("status_id")] long StatusId,
        [property: JsonPropertyName("is_active")] bool IsActive,
        [property: JsonPropertyName("last_activity")] DateTime? LastActivity,
        [property: JsonPropertyName("last_detection_time")] DateTime? LastDetectionTime,
        [property: JsonPropertyName("minutes_since_detection")] decimal? MinutesSinceDetection
    );

    // ── Plate hits ────────────────────────────────────────────────────────────

    public record AnprPlateHitsResponse(
        [property: JsonPropertyName("blacklist_today")] long BlacklistToday,
        [property: JsonPropertyName("whitelist_today")] long WhitelistToday,
        [property: JsonPropertyName("graylist_today")] long GraylistToday,
        [property: JsonPropertyName("recent_hits")] IReadOnlyList<AnprHitResponse>? RecentHits
    );

    public record AnprHitResponse(
        [property: JsonPropertyName("hit_id")] long HitId,
        [property: JsonPropertyName("hit_time")] DateTime HitTime,
        [property: JsonPropertyName("list_type")] string? ListType,
        [property: JsonPropertyName("severity")] string? Severity,
        [property: JsonPropertyName("plate_code")] string? PlateCode,
        [property: JsonPropertyName("plate_number")] string? PlateNumber,
        [property: JsonPropertyName("full_plate")] string? FullPlate,
        [property: JsonPropertyName("reason")] string? Reason,
        [property: JsonPropertyName("detection_id")] long? DetectionId,
        [property: JsonPropertyName("access_point_id")] long? AccessPointId,
        [property: JsonPropertyName("action_taken")] string? ActionTaken,
        [property: JsonPropertyName("resolved")] bool Resolved
    );

    // ── Selected period ───────────────────────────────────────────────────────

    public record AnprSelectedPeriodResponse(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("label")] string? Label,
        [property: JsonPropertyName("compare_label")] string? CompareLabel,
        [property: JsonPropertyName("from")] DateOnly? From,
        [property: JsonPropertyName("to")] DateOnly? To,
        [property: JsonPropertyName("week_start")] DateOnly? WeekStart,
        [property: JsonPropertyName("week_end")] DateOnly? WeekEnd,
        [property: JsonPropertyName("month_start")] DateOnly? MonthStart,
        [property: JsonPropertyName("month_end")] DateOnly? MonthEnd,
        [property: JsonPropertyName("month_name")] string? MonthName
    );

    // ── Metadata ──────────────────────────────────────────────────────────────

    public record AnprMetadataResponse(
        [property: JsonPropertyName("processing_time_ms")] decimal? ProcessingTimeMs
    );

    // ── Recent detections ─────────────────────────────────────────────────────

    public record AnprRecentDetectionsWrapperResponse(
        [property: JsonPropertyName("success")] bool Success,
        [property: JsonPropertyName("message")] string? Message,
        [property: JsonPropertyName("data")] IReadOnlyList<RecentDetectionResponse>? Data,
        [property: JsonPropertyName("metadata")] AnprRecentDetectionsMetadataResponse? Metadata
    );

    public record AnprRecentDetectionsMetadataResponse(
        [property: JsonPropertyName("limit")] long? Limit,
        [property: JsonPropertyName("workspace_id")] long? WorkspaceId,
        [property: JsonPropertyName("timestamp")] DateTime? Timestamp,
        [property: JsonPropertyName("filters")] AnprRecentDetectionsFiltersResponse? Filters
    );

    public record AnprRecentDetectionsFiltersResponse(
        [property: JsonPropertyName("zone_id")] long? ZoneId,
        [property: JsonPropertyName("project_id")] long? ProjectId,
        [property: JsonPropertyName("hardware_id")] long? HardwareId,
        [property: JsonPropertyName("min_confidence")] decimal? MinConfidence,
        [property: JsonPropertyName("access_point_id")] long? AccessPointId,
        [property: JsonPropertyName("project_area_id")] long? ProjectAreaId
    );

    public record RecentDetectionResponse(
        [property: JsonPropertyName("detection_id")] long DetectionId,
        [property: JsonPropertyName("detection_time")] DateTime DetectionTime,
        [property: JsonPropertyName("plate_code")] string? PlateCode,
        [property: JsonPropertyName("plate_number")] string? PlateNumber,
        [property: JsonPropertyName("full_plate")] string? FullPlate,
        [property: JsonPropertyName("confidence_score")] decimal ConfidenceScore,
        [property: JsonPropertyName("vehicle_color")] string? VehicleColor,
        [property: JsonPropertyName("vehicle_type")] string? VehicleType,
        [property: JsonPropertyName("vehicle_make")] string? VehicleMake,
        [property: JsonPropertyName("vehicle_model")] string? VehicleModel,
        [property: JsonPropertyName("direction")] string? Direction,
        [property: JsonPropertyName("lane_number")] long? LaneNumber,
        [property: JsonPropertyName("is_validated")] bool IsValidated,
        [property: JsonPropertyName("is_blacklisted")] bool IsBlacklisted,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("image_url")] string? ImageUrl,
        [property: JsonPropertyName("plate_crop_url")] string? PlateCropUrl,
        [property: JsonPropertyName("latitude")] decimal? Latitude,
        [property: JsonPropertyName("longitude")] decimal? Longitude,
        [property: JsonPropertyName("access_point")] RecentDetectionAccessPointResponse? AccessPoint,
        [property: JsonPropertyName("hardware")] RecentDetectionHardwareResponse? Hardware,
        [property: JsonPropertyName("project")] RecentDetectionProjectResponse? Project,
        [property: JsonPropertyName("zone")] RecentDetectionZoneResponse? Zone,
        [property: JsonPropertyName("area")] RecentDetectionAreaResponse? Area,
        [property: JsonPropertyName("country")] RecentDetectionCountryResponse? Country,
        [property: JsonPropertyName("state")] RecentDetectionStateResponse? State,
        [property: JsonPropertyName("category")] RecentDetectionCategoryResponse? Category
    );

    public record RecentDetectionAccessPointResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("prefix")] string? Prefix,
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("latitude")] decimal? Latitude,
        [property: JsonPropertyName("longitude")] decimal? Longitude,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );

    public record RecentDetectionHardwareResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("manufacturer")] string? Manufacturer,
        [property: JsonPropertyName("model")] string? Model,
        [property: JsonPropertyName("serial_number")] string? SerialNumber,
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("hardware_type")] string? HardwareType,
        [property: JsonPropertyName("firmware_version")] string? FirmwareVersion
    );

    public record RecentDetectionProjectResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );

    public record RecentDetectionZoneResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("code")] string? Code,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );

    public record RecentDetectionAreaResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );

    public record RecentDetectionCountryResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );

    public record RecentDetectionStateResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );

    public record RecentDetectionCategoryResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
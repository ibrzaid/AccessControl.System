using System.Text.Json.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ACS.Models.Response.V1.ANPRService.Seach
{
    public record AnprSearchResponse(
        bool Success,
        string Message,
        string ErrorCode,
        string RequestId,
        [property: JsonPropertyName("data")] IReadOnlyList<AnprSearchDetectionResponse>? Data,
        [property: JsonPropertyName("pagination")] AnprSearchPaginationResponse? Pagination,
        [property: JsonPropertyName("map_features")] AnprSearchMapFeaturesResponse? MapFeatures,
        [property: JsonPropertyName("filters")] AnprSearchFiltersResponse? Filters,
        [property: JsonPropertyName("metadata")] AnprSearchMetadataResponse? Metadata
    ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    // ── Pagination ────────────────────────────────────────────────────────────

    public record AnprSearchPaginationResponse(
        [property: JsonPropertyName("page")] int Page,
        [property: JsonPropertyName("page_size")] int PageSize,
        [property: JsonPropertyName("total_count")] long TotalCount,
        [property: JsonPropertyName("total_pages")] long TotalPages
    );

    // ── Metadata ──────────────────────────────────────────────────────────────

    public record AnprSearchMetadataResponse(
        [property: JsonPropertyName("timezone")] string? Timezone,
        [property: JsonPropertyName("processing_time_ms")] decimal? ProcessingTimeMs,
        [property: JsonPropertyName("timestamp")] DateTime? Timestamp
    );

    // =========================================================================
    // Detection row — one item in data[]
    // Maps every field returned by the function for a single detection.
    // =========================================================================

    public record AnprSearchDetectionResponse(
        // ── Core ──────────────────────────────────────────────────────────────
        [property: JsonPropertyName("detection_id")] long DetectionId,
        [property: JsonPropertyName("detection_time")] DateTime DetectionTime,
        [property: JsonPropertyName("detection_time_utc")] DateTime? DetectionTimeUtc,
        [property: JsonPropertyName("age_seconds")] int? AgeSeconds,

        // ── Plate ─────────────────────────────────────────────────────────────
        [property: JsonPropertyName("plate_code")] string? PlateCode,
        [property: JsonPropertyName("plate_number")] string? PlateNumber,
        [property: JsonPropertyName("full_plate")] string? FullPlate,

        // ── Confidence ────────────────────────────────────────────────────────
        [property: JsonPropertyName("confidence_score")] decimal ConfidenceScore,
        [property: JsonPropertyName("confidence_details")] System.Text.Json.JsonElement? ConfidenceDetails,

        // ── Images ────────────────────────────────────────────────────────────
        [property: JsonPropertyName("image_url")] string? ImageUrl,
        [property: JsonPropertyName("plate_crop_url")] string? PlateCropUrl,

        // ── Vehicle ───────────────────────────────────────────────────────────
        [property: JsonPropertyName("vehicle_color")] string? VehicleColor,
        [property: JsonPropertyName("vehicle_make")] string? VehicleMake,
        [property: JsonPropertyName("vehicle_model")] string? VehicleModel,
        [property: JsonPropertyName("vehicle_type")] string? VehicleType,
        [property: JsonPropertyName("vehicle_speed_kmh")] decimal? VehicleSpeedKmh,

        // ── Detection metadata ────────────────────────────────────────────────
        [property: JsonPropertyName("direction")] string? Direction,
        [property: JsonPropertyName("lane_number")] int? LaneNumber,
        [property: JsonPropertyName("latitude")] decimal? Latitude,
        [property: JsonPropertyName("longitude")] decimal? Longitude,
        [property: JsonPropertyName("anpr_engine")] string? AnprEngine,
        [property: JsonPropertyName("processing_time_ms")] int? ProcessingTimeMs,

        // ── Validation ────────────────────────────────────────────────────────
        [property: JsonPropertyName("is_validated")] bool IsValidated,
        [property: JsonPropertyName("validated_at")] DateTime? ValidatedAt,
        [property: JsonPropertyName("validated_at_utc")] DateTime? ValidatedAtUtc,
        [property: JsonPropertyName("validated_by")] long? ValidatedBy,
        [property: JsonPropertyName("validation_notes")] string? ValidationNotes,
        [property: JsonPropertyName("tags")] IReadOnlyList<string>? Tags,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,

        // ── Blacklist (NEW — added in updated function) ───────────────────────
        [property: JsonPropertyName("is_blacklisted")] bool IsBlacklisted,
        [property: JsonPropertyName("blacklist_details")] AnprSearchBlacklistResponse? BlacklistDetails,

        // ── Related objects ───────────────────────────────────────────────────
        [property: JsonPropertyName("hardware")] AnprSearchHardwareResponse? Hardware,
        [property: JsonPropertyName("access_point")] AnprSearchAccessPointResponse? AccessPoint,
        [property: JsonPropertyName("project")] AnprSearchNamedRefResponse? Project,
        [property: JsonPropertyName("area")] AnprSearchNamedRefResponse? Area,
        [property: JsonPropertyName("zone")] AnprSearchZoneResponse? Zone,
        [property: JsonPropertyName("country")] AnprSearchCountryResponse? Country,
        [property: JsonPropertyName("state")] AnprSearchNamedRefResponse? State,
        [property: JsonPropertyName("category")] AnprSearchNamedRefResponse? Category,
        [property: JsonPropertyName("validated_by_user")] AnprSearchUserResponse? ValidatedByUser
    );

    // ── Blacklist details ─────────────────────────────────────────────────────

    public record AnprSearchBlacklistResponse(
        [property: JsonPropertyName("list_type")] string? ListType,
        [property: JsonPropertyName("reason")] string? Reason,
        [property: JsonPropertyName("severity")] string? Severity,
        [property: JsonPropertyName("action_on_detect")] string? ActionOnDetect,
        [property: JsonPropertyName("reference_number")] string? ReferenceNumber
    );

    // ── Hardware ──────────────────────────────────────────────────────────────

    public record AnprSearchHardwareResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("serial_number")] string? SerialNumber,
        [property: JsonPropertyName("manufacturer")] string? Manufacturer,
        [property: JsonPropertyName("model")] string? Model,
        [property: JsonPropertyName("hardware_type")] string? HardwareType,
        [property: JsonPropertyName("status")] string? Status
    );

    // ── Access Point ──────────────────────────────────────────────────────────

    public record AnprSearchAccessPointResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name,
        [property: JsonPropertyName("prefix")] string? Prefix,
        [property: JsonPropertyName("latitude")] decimal? Latitude,
        [property: JsonPropertyName("longitude")] decimal? Longitude,
        [property: JsonPropertyName("type")] string? Type,          // ENTRY | EXIT | CAR
        [property: JsonPropertyName("orientation")] int? Orientation
    );

    // ── Zone (has code field unlike area/project/category) ───────────────────

    public record AnprSearchZoneResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("code")] string? Code,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );

    // ── Country (has code field) ──────────────────────────────────────────────

    public record AnprSearchCountryResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("code")] string? Code,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );

    // ── Generic named reference (project, area, state, category) ─────────────

    public record AnprSearchNamedRefResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("name")] Dictionary<string, string>? Name
    );

    // ── Validated by user ─────────────────────────────────────────────────────

    public record AnprSearchUserResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("username")] string? Username,
        [property: JsonPropertyName("full_name")] string? FullName
    );

    // =========================================================================
    // Map Features — GeoJSON FeatureCollection
    // Only present in response when pin_include_map_features = TRUE.
    //
    // Features array contains three types:
    //   type = 'detection'           → Point at detection lat/lng
    //   type = 'path'                → LineString (single plate search only)
    //   type = 'access_point_visit'  → Point at access point lat/lng
    // =========================================================================

    public record AnprSearchMapFeaturesResponse(
        [property: JsonPropertyName("type")] string? Type,              // "FeatureCollection"
        [property: JsonPropertyName("features")] IReadOnlyList<AnprSearchFeatureResponse>? Features,
        [property: JsonPropertyName("is_single_plate")] bool IsSinglePlate,
        [property: JsonPropertyName("has_path")] bool HasPath
    );

    public record AnprSearchFeatureResponse(
        [property: JsonPropertyName("type")] string? Type,      // "Feature"
        [property: JsonPropertyName("geometry")] AnprSearchGeometryResponse? Geometry,
        [property: JsonPropertyName("properties")] AnprSearchFeaturePropertiesResponse? Properties
    );

    public record AnprSearchGeometryResponse(
        [property: JsonPropertyName("type")] string? Type,         // Point | LineString
        [property: JsonPropertyName("coordinates")] System.Text.Json.JsonElement? Coordinates   // [lng,lat] or [[lng,lat],...]
    );

    public record AnprSearchFeaturePropertiesResponse(
        // ── Common ────────────────────────────────────────────────────────────
        [property: JsonPropertyName("type")] string? Type,          // detection | path | access_point_visit
        [property: JsonPropertyName("marker_color")] string? MarkerColor,   // hex color pre-computed by DB

        // ── Detection point ───────────────────────────────────────────────────
        [property: JsonPropertyName("id")] long? Id,
        [property: JsonPropertyName("time")] DateTime? Time,
        [property: JsonPropertyName("age_seconds")] int? AgeSeconds,
        [property: JsonPropertyName("plate")] string? Plate,
        [property: JsonPropertyName("plate_code")] string? PlateCode,
        [property: JsonPropertyName("plate_number")] string? PlateNumber,
        [property: JsonPropertyName("confidence")] decimal? Confidence,
        [property: JsonPropertyName("direction")] string? Direction,
        [property: JsonPropertyName("lane_number")] int? LaneNumber,
        [property: JsonPropertyName("is_blacklisted")] bool? IsBlacklisted,
        [property: JsonPropertyName("is_validated")] bool? IsValidated,
        [property: JsonPropertyName("image_url")] string? ImageUrl,
        [property: JsonPropertyName("plate_crop_url")] string? PlateCropUrl,
        [property: JsonPropertyName("vehicle_color")] string? VehicleColor,
        [property: JsonPropertyName("vehicle_type")] string? VehicleType,
        [property: JsonPropertyName("speed_kmh")] decimal? SpeedKmh,
        [property: JsonPropertyName("bl_severity")] string? BlSeverity,
        [property: JsonPropertyName("bl_reason")] string? BlReason,

        // ── Path (LineString) ─────────────────────────────────────────────────
        [property: JsonPropertyName("start_time")] DateTime? StartTime,
        [property: JsonPropertyName("end_time")] DateTime? EndTime,
        [property: JsonPropertyName("total_stops")] long? TotalStops,

        // ── Access point visit ────────────────────────────────────────────────
        [property: JsonPropertyName("name")] System.Text.Json.JsonElement? Name,
        [property: JsonPropertyName("prefix")] string? Prefix,
        [property: JsonPropertyName("type_code")] string? TypeCode,      // ENTRY | EXIT | CAR
        [property: JsonPropertyName("orientation")] int? Orientation,
        [property: JsonPropertyName("visit_count")] long? VisitCount,
        [property: JsonPropertyName("first_seen")] DateTime? FirstSeen,
        [property: JsonPropertyName("last_seen")] DateTime? LastSeen,
        [property: JsonPropertyName("avg_confidence")] decimal? AvgConfidence,

        // ── Nested access point (on detection points) ─────────────────────────
        [property: JsonPropertyName("access_point")] AnprSearchFeatureAccessPointResponse? AccessPoint
    );

    public record AnprSearchFeatureAccessPointResponse(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("name")] System.Text.Json.JsonElement? Name,
        [property: JsonPropertyName("prefix")] string? Prefix,
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("orientation")] int? Orientation
    );

    // =========================================================================
    // Filters echo — mirrors all input params back in the response
    // =========================================================================

    public record AnprSearchFiltersResponse(
        [property: JsonPropertyName("workspace_id")] long? WorkspaceId,
        [property: JsonPropertyName("user_id")] long? UserId,
        [property: JsonPropertyName("project_id")] long? ProjectId,
        [property: JsonPropertyName("project_area_id")] long? ProjectAreaId,
        [property: JsonPropertyName("zone_id")] long? ZoneId,
        [property: JsonPropertyName("access_point_id")] long? AccessPointId,
        [property: JsonPropertyName("hardware_id")] long? HardwareId,
        [property: JsonPropertyName("plate_code")] string? PlateCode,
        [property: JsonPropertyName("plate_number")] string? PlateNumber,
        [property: JsonPropertyName("from_date")] DateTime? FromDate,
        [property: JsonPropertyName("to_date")] DateTime? ToDate,
        [property: JsonPropertyName("min_confidence")] decimal? MinConfidence,
        [property: JsonPropertyName("max_confidence")] decimal? MaxConfidence,
        [property: JsonPropertyName("country_id")] long? CountryId,
        [property: JsonPropertyName("state_id")] long? StateId,
        [property: JsonPropertyName("category_id")] long? CategoryId,
        [property: JsonPropertyName("direction")] string? Direction,
        [property: JsonPropertyName("lane_number")] int? LaneNumber,
        [property: JsonPropertyName("vehicle_type")] string? VehicleType,
        [property: JsonPropertyName("vehicle_color")] string? VehicleColor,
        [property: JsonPropertyName("vehicle_make")] string? VehicleMake,
        [property: JsonPropertyName("is_validated")] bool? IsValidated,
        [property: JsonPropertyName("is_blacklisted")] bool? IsBlacklisted,
        [property: JsonPropertyName("sort_by")] string? SortBy,
        [property: JsonPropertyName("sort_dir")] string? SortDir,
        [property: JsonPropertyName("include_map_features")] bool? IncludeMapFeatures
    );
}
#pragma warning restore IDE0130 // Namespace does not match folder structure
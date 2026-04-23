#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService.Dashboard
{
    public record SetupDashboardResponse(
    bool Success,
     string? Message,
     string ErrorCode,
     string RequestId,
     [property: JsonPropertyName("generated_at")] DateTime? GeneratedAt,
     [property: JsonPropertyName("detail")] string? Detail,
     [property: JsonPropertyName("user_context")] UserContextResponse? UserContext,
     [property: JsonPropertyName("cameras")] CamerasResponse? Cameras,
     [property: JsonPropertyName("projects")] ProjectCardResponse[]? Projects
) : BaseResponses(Success, Message, ErrorCode, RequestId);


    public record UserContextResponse(
        [property: JsonPropertyName("user_id")] long UserId,
        [property: JsonPropertyName("full_name")] string FullName,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("avatar_url")] string? AvatarUrl,
        [property: JsonPropertyName("language")] string? Language,
        [property: JsonPropertyName("timezone")] string? Timezone,
        [property: JsonPropertyName("last_login")] DateTime? LastLogin,
        [property: JsonPropertyName("workspace_id")] long WorkspaceId,
        [property: JsonPropertyName("workspace_name")] string WorkspaceName,
        [property: JsonPropertyName("workspace_code")] string WorkspaceCode,
        [property: JsonPropertyName("scope_type_id")] int ScopeTypeId,
        [property: JsonPropertyName("project_ids")] long[]? ProjectIds,
        [property: JsonPropertyName("modules")] ModulesResponse? Modules
    );

    public record ModulesResponse(
        [property: JsonPropertyName("anpr")] bool Anpr,
        [property: JsonPropertyName("parking")] bool Parking,
        [property: JsonPropertyName("gates")] bool Gates,
        [property: JsonPropertyName("prebooking")] bool Prebooking,
        [property: JsonPropertyName("blacklist")] bool Blacklist
    );

    public record CamerasResponse(
        [property: JsonPropertyName("online")] int Online,
        [property: JsonPropertyName("offline")] int Offline,
        [property: JsonPropertyName("warning")] int Warning,
        [property: JsonPropertyName("total")] int Total,
        [property: JsonPropertyName("uptime_pct")] decimal UptimePct,
        [property: JsonPropertyName("list")] CameraListItemResponse[]? List
    );

    public record CameraListItemResponse(
        [property: JsonPropertyName("hardware_id")] long HardwareId,
        [property: JsonPropertyName("model")] string? Model,
        [property: JsonPropertyName("manufacturer")] string? Manufacturer,
        [property: JsonPropertyName("status")] string Status,      // ONLINE | OFFLINE | WARNING | MAINTENANCE
        [property: JsonPropertyName("access_point")] string? AccessPoint,
        [property: JsonPropertyName("last_activity")] DateTime? LastActivity,
        [property: JsonPropertyName("detection_count")] long DetectionCount,
        [property: JsonPropertyName("project_id")] long ProjectId,
        [property: JsonPropertyName("project_area_id")] long ProjectAreaId,
        [property: JsonPropertyName("zone_id")] long ZoneId
    );

    /// <summary>
    /// Returned by the setup service. DetectionsToday and ActiveSessions are
    /// null until enriched from the ANPR and Parking service responses.
    /// </summary>
    public record ProjectCardResponse(
        [property: JsonPropertyName("project_id")] long ProjectId,
        [property: JsonPropertyName("project_name")] string ProjectName,
        [property: JsonPropertyName("project_names")] Dictionary<string, string>? ProjectNames,
        [property: JsonPropertyName("city")] string? City,
        [property: JsonPropertyName("cameras_online")] int CamerasOnline,
        [property: JsonPropertyName("cameras_total")] int CamerasTotal,
        [property: JsonPropertyName("detections_today")] long? DetectionsToday,  // enriched by ANPR
        [property: JsonPropertyName("active_sessions")] long? ActiveSessions    // enriched by Parking
    );

    /// <summary>
    /// scope_type_id integer values from tbl_user_access.
    /// </summary>
    public static class ScopeTypeId
    {
        public const int Zone = 1;
        public const int ProjectArea = 2;
        public const int Project = 3;
        public const int Workspace = 4;
        public const int AccessPoint = 5;

        public static string Label(int id) => id switch
        {
            Workspace => "Workspace Admin",
            Project => "Project Manager",
            ProjectArea => "Area Supervisor",
            Zone => "Zone Operator",
            AccessPoint => "Gate / Cashier",
            _ => "User"
        };
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure

using ACS.Helper.V1;
using Asp.Versioning;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using ACS.Models.Response.V1.ANPRService.Live;
using ACS.ANPR.WebService.Services.V1.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ACS.ANPR.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class LiveController(ILiveService service, FindClaimHelper findClaimHelper) : BaseController
    {
        // ── Error factory — positional record constructor, all optional fields null ──
        private static LiveResponse Fail(string? requestId, string code, string message) => new(
            /* Success          */ false,
            /* Message          */ message,
            /* ErrorCode        */ code,
            /* RequestId        */ requestId ?? "",
            /* CurrentLevel     */ null,
            /* AvailableViews   */ null,
            /* Breadcrumb       */ null,
            /* HierarchyIds     */ null,
            /* Master           */ null,
            /* Type             */ null,
            /* Features         */ null,
            /* Total            */ null,
            /* Bounds           */ null,
            /* Data             */ null,
            /* Hotspots         */ null,
            /* LiveConfig       */ null,
            /* Stats            */ null,
            /* Pagination       */ null,
            /* Timestamp        */ null,
            /* RefreshInterval  */ null,
            /* ZoneInfo         */ null,
            /* AccessPoint      */ null,
            /* TotalHardware    */ null,
            /* Hardware         */ null,
            /* RecentDetections */ null,
            /* Camera           */ null,
            /* Center           */ null,
            /* Radius           */ null,
            /* Filters          */ null,
            /* Metadata         */ null);

        /// <summary>
        /// Get map data for the requested hierarchy level and/or overlay view type.
        /// </summary>
        [HttpGet]
        [Route("view")]
        [MapToApiVersion("1.0")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(LiveResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LiveResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LiveResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LiveResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMapViewAsync(

            // ── View type
            [FromQuery(Name = "view_type")]
            [RegularExpression(
                @"^(auto|projects|areas|zones|zone_children|access_point|hardware|live|heatmap|live_heatmap|clusters|tracking|3d|satellite)$",
                ErrorMessage = "view_type must be one of: auto, projects, areas, zones, zone_children, access_point, hardware, live, heatmap, live_heatmap, clusters, tracking, 3d, satellite")]
            string? viewType = "auto",

            // ── Hierarchy IDs
            [FromQuery(Name = "project_id")][Range(1, int.MaxValue)] int? projectId = null,
            [FromQuery(Name = "area_id")][Range(1, int.MaxValue)] int? areaId = null,
            [FromQuery(Name = "zone_id")][Range(1, long.MaxValue)] long? zoneId = null,
            [FromQuery(Name = "access_point_id")][Range(1, int.MaxValue)] int? accessPointId = null,
            [FromQuery(Name = "hardware_id")][Range(1, int.MaxValue)] int? hardwareId = null,

            // ── Spatial
            [FromQuery(Name = "zoom_level")][Range(0, 22)] int zoomLevel = 12,
            [FromQuery(Name = "center_lat")][Range(-90, 90)] double? centerLat = null,
            [FromQuery(Name = "center_lng")][Range(-180, 180)] double? centerLng = null,

            [FromQuery(Name = "bounds_south")][Range(-90, 90)] double? boundsSouth = null,
            [FromQuery(Name = "bounds_north")][Range(-90, 90)] double? boundsNorth = null,
            [FromQuery(Name = "bounds_west")][Range(-180, 180)] double? boundsWest = null,
            [FromQuery(Name = "bounds_east")][Range(-180, 180)] double? boundsEast = null,

            // ── Time window
            [FromQuery(Name = "from_time")] DateTime? fromTime = null,
            [FromQuery(Name = "to_time")] DateTime? toTime = null,
            //[FromQuery(Name = "window_minutes")][Range(1, 1440)] int windowMinutes = 5,
            [FromQuery(Name = "window_minutes")][Range(1, int.MaxValue)] int windowMinutes = 5,

            // ── Detection filters
            [FromQuery(Name = "min_confidence")][Range(0, 100)] double minConfidence = 0,
            [FromQuery(Name = "resolution")][Range(10, 200)] int resolution = 50,
            [FromQuery(Name = "cluster_radius")][Range(10, 500)] int clusterRadius = 50,
            [FromQuery(Name = "weight_by_confidence")] bool weightByConfidence = true,
            [FromQuery(Name = "include_trend")] bool includeTrend = true,

            // ── Tracking
            [FromQuery(Name = "plate_code")][StringLength(10)] string? plateCode = null,
            [FromQuery(Name = "plate_number")][StringLength(20)] string? plateNumber = null,
            [FromQuery(Name = "include_path")] bool includePath = true,
            [FromQuery(Name = "include_stops")] bool includeStops = true,

            // ── 3D
            [FromQuery(Name = "height_factor")][Range(0.1, 10.0)] double heightFactor = 1.0,

            // ── Pagination
            [FromQuery(Name = "limit")][Range(1, 10000)] int limit = 500,
            [FromQuery(Name = "offset")][Range(0, int.MaxValue)] int offset = 0)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                        .ToList();
                    if (errors.Count != 0)
                        return BadRequest(Fail(null, "VALIDATION_ERROR", string.Join(", ", errors)));
                }

                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(workspace) || !int.TryParse(workspace, out _))
                    return Unauthorized(Fail(requestId, "INVALID_WORKSPACE",
                        "Invalid or missing workspace identifier"));

                // ── Tracking guard
                var effectiveViewType = viewType ?? "auto";
                if (effectiveViewType == "tracking" &&
                    (string.IsNullOrEmpty(plateCode) || string.IsNullOrEmpty(plateNumber)))
                    return BadRequest(Fail(requestId, "TRACKING_REQUIREMENTS_MISSING",
                        "Tracking view requires both plate_code and plate_number"));

                // ── Bounds guard
                bool hasBounds = boundsSouth.HasValue && boundsNorth.HasValue &&
                                 boundsWest.HasValue  && boundsEast.HasValue;
                if (hasBounds && (boundsSouth!.Value > boundsNorth!.Value ||
                                  boundsWest!.Value  > boundsEast!.Value))
                    return BadRequest(Fail(requestId, "INVALID_BOUNDS",
                        "Bounds are invalid: south must be ≤ north and west must be ≤ east"));

                var response = await service.GetMapViewAsync(
                    viewType: effectiveViewType,
                    projectId: projectId,
                    areaId: areaId,
                    zoneId: zoneId,
                    accessPointId: accessPointId,
                    hardwareId: hardwareId,
                    zoomLevel: zoomLevel,
                    centerLat: centerLat,
                    centerLng: centerLng,
                    boundsSouth: boundsSouth,
                    boundsNorth: boundsNorth,
                    boundsWest: boundsWest,
                    boundsEast: boundsEast,
                    fromTime: fromTime,
                    toTime: toTime,
                    windowMinutes: windowMinutes,
                    minConfidence: minConfidence,
                    resolution: resolution,
                    clusterRadius: clusterRadius,
                    weightByConfidence: weightByConfidence,
                    includeTrend: includeTrend,
                    plateCode: plateCode,
                    plateNumber: plateNumber,
                    includePath: includePath,
                    includeStops: includeStops,
                    heightFactor: heightFactor,
                    limit: limit,
                    offset: offset,
                    workspaceId: workspace,
                    user: user,
                    ipAddress: ipAddress,
                    agent: serAgent,
                    requestId: requestId,
                    cancellationToken: HttpContext.RequestAborted);

                if (!response.Success) return response.ErrorCode switch
                {
                    "INVALID_WORKSPACE" or "UNAUTHORIZED" => Unauthorized(response),
                    "WORKSPACE_NOT_FOUND" or "HIERARCHY_NOT_FOUND" => NotFound(response),
                    "INTERNAL_ERROR" => StatusCode(500, response),
                    _ => BadRequest(response)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
using ACS.Helper.V1;
using Asp.Versioning;
using ACS.Models.Response;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using ACS.Models.Response.V1.ANPRService.Seach;
using ACS.ANPR.WebService.Services.V1.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ACS.ANPR.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class SearchController(ISearchService service, FindClaimHelper findClaimHelper) : BaseController
    {

        private static AnprSearchResponse Fail(string? requestId, string code, string message) =>
           new(Success: false,
               Message: message,
               ErrorCode: code,
               RequestId: requestId ?? "",
               Data: null,
               Pagination: null,
               MapFeatures: null,
               Filters: null,
               Metadata: null);


        /// <summary>
        /// Search ANPR detections with full filter, sort and pagination support.
        /// Results include flat rows for list/grid view.
        /// Pass include_map_features=true to also get a GeoJSON FeatureCollection
        /// ready for map display (detection dots, journey path, access point markers).
        /// </summary>
        /// <remarks>
        /// **Hierarchy scope** — narrow results by passing any combination:
        /// project_id → area_id → zone_id → access_point_id → hardware_id
        ///
        /// **Plate search** — LIKE on both fields independently:
        /// - plate_code=KSA matches any code containing "KSA"
        /// - plate_number=123 matches any number containing "123"
        ///
        /// **Timezone** — resolved automatically:
        /// user preference → project timezone → workspace timezone → UTC
        /// All timestamps in the response are in the resolved local timezone.
        ///
        /// **Map features** — set include_map_features=true when the map tab is active:
        /// - One Point per detection (marker_color pre-computed)
        /// - LineString path when an exact single plate is searched (≥ 2 points)
        /// - Access point visit markers (deduplicated)
        /// </remarks>
        /// <response code="200">Search completed successfully.</response>
        /// <response code="400">Validation error — invalid parameter value.</response>
        /// <response code="401">Missing or invalid authentication token.</response>
        /// <response code="500">Unexpected server-side error.</response>
        [HttpGet]
        [Route("search")]
        [MapToApiVersion("1.0")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(AnprSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AnprSearchResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchAsync(
            [FromQuery(Name = "project_id")][Range(1, int.MaxValue)]int? projectId = null,
            [FromQuery(Name = "area_id")][Range(1, int.MaxValue)]int? areaId = null,
            [FromQuery(Name = "zone_id")] [Range(1, long.MaxValue)]long? zoneId = null,
            [FromQuery(Name = "access_point_id")][Range(1, int.MaxValue)]int? accessPointId = null,
            [FromQuery(Name = "hardware_id")][Range(1, int.MaxValue)] int? hardwareId = null,
            [FromQuery(Name = "plate_code")] [StringLength(10)]string? plateCode = null,
            [FromQuery(Name = "plate_number")][StringLength(20)]string? plateNumber = null,
            [FromQuery(Name = "from_time")]DateTime? fromTime = null,
            [FromQuery(Name = "to_time")]DateTime? toTime = null,
            [FromQuery(Name = "min_confidence")][Range(0, 100)] double? minConfidence = null,
            [FromQuery(Name = "max_confidence")][Range(0, 100)]double? maxConfidence = null,
            [FromQuery(Name = "country_id")][Range(1, int.MaxValue)]int? countryId = null,
            [FromQuery(Name = "state_id")][Range(1, int.MaxValue)] int? stateId = null,
            [FromQuery(Name = "category_id")][Range(1, int.MaxValue)]int? categoryId = null,
            [FromQuery(Name = "direction")] [RegularExpression(@"^(ENTER|EXIT|UNKNOWN)$", ErrorMessage = "direction must be ENTER, EXIT, or UNKNOWN")]string? direction = null,
            [FromQuery(Name = "lane_number")][Range(1, 99)] int? laneNumber = null,
            [FromQuery(Name = "vehicle_type")] [StringLength(50)]string? vehicleType = null,
            [FromQuery(Name = "vehicle_color")][StringLength(50)]string? vehicleColor = null,
            [FromQuery(Name = "vehicle_make")][StringLength(100)] string? vehicleMake = null,
            [FromQuery(Name = "is_validated")]bool? isValidated = null,
            [FromQuery(Name = "is_blacklisted")]bool? isBlacklisted = null,
            [FromQuery(Name = "sort_by")] [RegularExpression(@"^(time|confidence|plate|speed)$", ErrorMessage = "sort_by must be: time, confidence, plate, or speed")]string? sortBy = "time",
            [FromQuery(Name = "sort_dir")] [RegularExpression(@"^(asc|desc)$", ErrorMessage = "sort_dir must be asc or desc")] string? sortDir = "desc",
            [FromQuery(Name = "include_map_features")] bool includeMapFeatures = false,
            [FromQuery(Name = "page")] [Range(1, int.MaxValue)]int page = 1,
            [FromQuery(Name = "page_size")][Range(1, 500)]int pageSize = 25)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)).ToList();
                    if (errors.Count > 0) return BadRequest(Fail(null, "VALIDATION_ERROR", string.Join(", ", errors)));
                }
                string requestId = GetRequestId();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(workspace) || !int.TryParse(workspace, out _))
                    return Unauthorized(Fail(requestId, "INVALID_WORKSPACE",
                        "Invalid or missing workspace identifier"));

                if (fromTime.HasValue && toTime.HasValue && fromTime.Value > toTime.Value)
                    return BadRequest(Fail(requestId, "INVALID_DATE_RANGE",
                        "from_time must be earlier than to_time"));

                if (minConfidence.HasValue && maxConfidence.HasValue
                    && minConfidence.Value > maxConfidence.Value)
                    return BadRequest(Fail(requestId, "INVALID_CONFIDENCE_RANGE",
                        "min_confidence must be less than or equal to max_confidence"));

                _=int.TryParse(user, out int userId);

                var response = await service.Search(
                    workspace: workspace,
                    requestId: requestId,
                    user: user,
                    project: projectId,
                    projectArea: areaId,
                    zone: zoneId,
                    accessPoint: accessPointId,
                    hardware: hardwareId,
                    plateCode: plateCode,
                    plateNumber: plateNumber,
                    fromDate: fromTime,
                    toDate: toTime,
                    minConfidence: minConfidence,
                    maxConfidence: maxConfidence,
                    country: countryId,
                    state: stateId,
                    category: categoryId,
                    direction: direction,
                    lane: laneNumber,
                    vehicleType: vehicleType,
                    vehicleColor: vehicleColor,
                    make: vehicleMake,
                    validated: isValidated,
                    blackListed: isBlacklisted,
                    sortBy: sortBy,
                    sortDir: sortDir,
                    includeMapFeatures: includeMapFeatures,
                    page: page,
                    pageSize: pageSize,
                    cancellationToken: HttpContext.RequestAborted);

                if (!response.Success) return response.ErrorCode switch
                {
                    "INVALID_WORKSPACE" or "UNAUTHORIZED" => Unauthorized(response),
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

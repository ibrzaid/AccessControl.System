using ACS.Helper.V1;
using Asp.Versioning;
using ACS.BusinessEntities;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.Models.Request.V1.SetupService.Zone;
using ACS.Models.Response.V1.SetupService.Zone;
using ACS.Setup.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.SetupService.AccessPoint;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ACS.Setup.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ZonesController(IZonesService service, IAccessPointService accessPointService,  FindClaimHelper findClaimHelper) : BaseController
    {

        /// <summary>
        /// Ge list of Access Points in a Zone with pagination. Caller must have at least read access to the zone to get the access points. If caller has write access to the zone, then canCreate flag will be true in the response which indicates that caller can create access point in the zone.
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpGet("{zoneId:int}/AccessPoints")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetAccessPointsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAccessPointsAsync(
            [FromRoute] int zoneId,
            [FromQuery] int limit = 100,
            [FromQuery] int offset = 0)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                var response  = await accessPointService.GetAccessPointsAsync(workspace, caller, zoneId, limit, offset, HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Create a new zone in a project area. Caller must have write access to the project area to create a zone. If the request contains parentZoneId, then the new zone will be created as a child zone under the parent zone. Otherwise, the new zone will be created as a top-level zone in the project area.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="201">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpPost]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationZoneResultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationZoneResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateZoneAsync([FromBody] CreateZoneRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationZoneResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.CreateZoneAsync(workspace, caller, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return StatusCode(201, response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Update an existing zone. Caller must have write access to the zone to update it. The request can update the zone name, zone code, total spots, grace period, geofence (center latitude, center longitude, and polygon coordinates) and active status of the zone. If the request contains geofence information, then the geofence of the zone will be updated. Otherwise, the geofence of the zone will remain unchanged.
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpPatch("{zoneId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationZoneResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationZoneResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateZoneAsync([FromRoute] int zoneId, [FromBody] UpdateZoneRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationZoneResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.UpdateZoneAsync(workspace, caller, zoneId, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Delete an existing zone. Caller must have write access to the zone to delete it. If the zone has child zones, then all the child zones will be deleted as well. If the zone has access points, then the access points will be unassigned from the zone but not deleted. The request must contain the current latitude and longitude of the zone to ensure that the caller is within the geofence of the zone or any of its parent zones. If the caller is outside the geofence, then the delete operation will be rejected. This is to prevent unauthorized deletion of zones by callers who are not physically present in the area.
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpDelete("{zoneId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationZoneResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationZoneResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteZoneAsync([FromRoute] int zoneId,
            [FromQuery] string latitude, [FromQuery] string longitude)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.DeleteZoneAsync(workspace, caller, zoneId, ip, ua, rid, latitude, longitude, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }
    }
}

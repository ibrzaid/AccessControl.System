using ACS.Helper.V1;
using Asp.Versioning;
using ACS.BusinessEntities;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.Setup.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.SetupService.Hardware; 
using ACS.Models.Request.V1.SetupService.AccessPoint;
using ACS.Models.Response.V1.SetupService.AccessPoint;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ACS.Setup.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AccessPointsController(IAccessPointService service,  IHardwareService hardwareService, FindClaimHelper findClaimHelper) : BaseController
    {
        /// <summary>
        /// Get hardware associated with an access point. This endpoint supports pagination through the 'limit' and 'offset' query parameters.
        /// </summary>
        /// <param name="accessPointId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpGet("{accessPointId:int}/Hardware")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetHardwareResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetHardwareAsync(
            [FromRoute] int accessPointId,
            [FromQuery] int limit = 100,
            [FromQuery] int offset = 0)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                var response  = await hardwareService.GetHardwareAsync(workspace, caller, accessPointId, limit, offset, HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Create a new access point. The request body should contain the necessary details for creating the access point, such as zone ID, access point name, prefix, serial number, access level ID, and optional latitude and longitude for geofencing. On successful creation, the endpoint returns the details of the newly created access point along with a success message. If there are validation errors or other issues during creation, appropriate error messages and status codes are returned.
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
        [ProducesResponseType(typeof(OperationAccessPointResultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationAccessPointResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateAccessPointAsync([FromBody] CreateAccessPointRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationAccessPointResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.CreateAccessPointAsync(workspace, caller, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return StatusCode(201, response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Update an existing access point. The endpoint accepts the access point ID as a route parameter and the updated details in the request body. The request body can include fields such as access point name, prefix, serial number, access level ID, and optional latitude and longitude for geofencing. On successful update, the endpoint returns the details of the updated access point along with a success message. If there are validation errors or other issues during the update process, appropriate error messages and status codes are returned.
        /// </summary>
        /// <param name="accessPointId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpPatch("{accessPointId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationAccessPointResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationAccessPointResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateAccessPointAsync([FromRoute] int accessPointId, [FromBody] UpdateAccessPointRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationAccessPointResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.UpdateAccessPointAsync(workspace, caller, accessPointId, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Delete an access point. The endpoint accepts the access point ID as a route parameter and optional latitude and longitude for geofencing as query parameters. On successful deletion, the endpoint returns a success message. If there are issues during the deletion process, such as validation errors or internal server errors, appropriate error messages and status codes are returned.
        /// </summary>
        /// <param name="accessPointId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpDelete("{accessPointId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationAccessPointResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationAccessPointResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteAccessPointAsync([FromRoute] int accessPointId,
             [FromQuery] string latitude, [FromQuery] string longitude)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.DeleteAccessPointAsync(workspace, caller, accessPointId, ip, ua, rid, latitude, longitude, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }
    }
}

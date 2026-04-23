using ACS.Helper.V1;
using Asp.Versioning;
using ACS.BusinessEntities;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.Models.Request.V1.SetupService.Area;
using ACS.Models.Response.V1.SetupService.Area;
using ACS.Models.Response.V1.SetupService.Zone;
using ACS.Setup.WebService.Services.V1.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ACS.Setup.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AreasController(IAreaService service, IZonesService zonesService, FindClaimHelper findClaimHelper) : BaseController
    {
        /// <summary>
        /// Get zones within a project area. This endpoint supports pagination through 'limit' and 'offset' query parameters.
        /// </summary>
        /// <param name="projectAreaId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpGet("{projectAreaId:int}/Zones")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetZonesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetZonesAsync(
            [FromRoute] int projectAreaId,
            [FromQuery] int limit = 100,
            [FromQuery] int offset = 0)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                var response  = await zonesService.GetZonesAsync(workspace, caller, projectAreaId, limit, offset, HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Create a new project area. The request body should contain the necessary details for the area to be created. On successful creation, it returns the details of the newly created area along with a success message. If the request is invalid or if there are any issues during the creation process, it returns an appropriate error message and status code.
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
        [ProducesResponseType(typeof(OperationAreaResultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationAreaResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateAreaAsync([FromBody] CreateAreaRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationAreaResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.CreateAreaAsync(workspace, caller, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return StatusCode(201, response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Update an existing project area. The endpoint accepts the project area ID as a route parameter and the updated details in the request body. It validates the input and updates the area accordingly. On success, it returns the updated area details along with a success message. If the request is invalid or if there are any issues during the update process, it returns an appropriate error message and status code.
        /// </summary>
        /// <param name="projectAreaId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpPatch("{projectAreaId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationAreaResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationAreaResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateAreaAsync([FromRoute] int projectAreaId, [FromBody] UpdateAreaRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationAreaResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.UpdateAreaAsync(workspace, caller, projectAreaId, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Delete a project area. The endpoint accepts the project area ID as a route parameter and the latitude and longitude as query parameters. It validates the input and deletes the area accordingly. On success, it returns a success message. If the request is invalid or if there are any issues during the deletion process, it returns an appropriate error message and status code.
        /// </summary>
        /// <param name="projectAreaId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpDelete("{projectAreaId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationAreaResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationAreaResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteAreaAsync([FromRoute] int projectAreaId,
            [FromQuery] string latitude, [FromQuery] string longitude)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.DeleteAreaAsync(workspace, caller, projectAreaId, ip, ua, rid, latitude, longitude, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }
    }
}

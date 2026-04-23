using ACS.Helper.V1;
using Asp.Versioning;
using ACS.BusinessEntities;
using System.Security.Claims;
using ACS.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.Models.Request.V1.SetupService.Hardware;
using ACS.Setup.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.SetupService.Hardware;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ACS.Setup.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class HardwareController(IHardwareService service, FindClaimHelper findClaimHelper) : BaseController
    {
        /// <summary>
        /// Create Hardware
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
        [ProducesResponseType(typeof(OperationHardwareResultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationHardwareResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateHardwareAsync([FromBody] CreateHardwareRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationHardwareResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.CreateHardwareAsync(workspace, caller, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return StatusCode(201, response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Update Hardware
        /// </summary>
        /// <param name="hardwareId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpPatch("{hardwareId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationHardwareResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationHardwareResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateHardwareAsync([FromRoute] int hardwareId, [FromBody] UpdateHardwareRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationHardwareResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.UpdateHardwareAsync(workspace, caller, hardwareId, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Delete Hardware
        /// </summary>
        /// <param name="hardwareId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpDelete("{hardwareId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationHardwareResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationHardwareResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteHardwareAsync([FromRoute] int hardwareId,
            [FromQuery] string latitude, [FromQuery] string longitude)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.DeleteHardwareAsync(workspace, caller, hardwareId, ip, ua, rid, latitude, longitude, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Test Hardware (connectivity / health-check probe).
        /// Stub: currently returns Success = true. Real device interrogation
        /// will be implemented later.
        /// </summary>
        /// <param name="hardwareId">Optional persisted hardware id. Pass 0 for create-mode tests.</param>
        /// <param name="request">Candidate hardware configuration to probe.</param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, probe acknowledged.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized.</response>
        /// <response code="500">HTTP Internal Server Error.</response>
        [HttpPost("{hardwareId:int}/test")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TestHardwareAsync([FromRoute] int hardwareId, [FromBody] TestHardwareRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new BaseResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                int? hwId = hardwareId > 0 ? hardwareId : null;
                var response = await service.TestHardwareAsync(workspace, caller, hwId, request, ip, ua, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Test Hardware (create-mode probe — no persisted id yet).
        /// </summary>
        [HttpPost("test")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public Task<IActionResult> TestHardwareDraftAsync([FromBody] TestHardwareRequest request) =>
            TestHardwareAsync(0, request);
    }
}

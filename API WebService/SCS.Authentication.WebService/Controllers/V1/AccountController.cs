using ACS.Helper.V1;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ACS.Models.Request.V1.AuthenticationService.Account;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.Account;

namespace ACS.Authentication.WebService.Controllers.V1
{

    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AccountController(IAuthenticationService service, FindClaimHelper findClaimHelper) : BaseController
    {

        /// <summary>
        /// Login user to the system
        /// </summary>
        /// <param name="request"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="403">HTTP Forbidden, Returned when the submitted request is invalid.</response>
        /// <response code="404">HTTP NotFound, Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [MapToApiVersion("1.0")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginAsync([FromForm] LoginRequest request,
            [FromHeader(Name = "client-id")][Required] string client = "app_web_application_1767709224_9884")
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
                    if (errors.Count!=0) return BadRequest(new LoginResponse { Error = $"{string.Join(",", errors)}", ErrorCode = "A01" });
                }
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                var response = await service.LoginAsync(request, client, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);
                if (!response.Success) return response.ErrorCode switch
                {
                    "INVALID_CREDENTIALS" or "INVALID_PASSWORD" => Unauthorized(response),
                    "ACCOUNT_LOCKED" or "ACCOUNT_INACTIVE" or "ACCOUNT_SUSPENDED" or "INVALID_CLIENT" or "INACTIVE_CLIENT" => StatusCode(403, response),
                    "LOGIN_ERROR" => StatusCode(500, response),
                    _ => BadRequest(response)
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        /// <summary>
        /// This Router is using to get new token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="403">HTTP Forbidden, Returned when the submitted request is invalid.</response>
        /// <response code="404">HTTP NotFound, Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpPost]
        [Route("refresh_token")]
        [AllowAnonymous]
        [MapToApiVersion("1.0")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshTokenAsync([FromForm] RefreshTokenRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
                    if (errors.Count!=0) return BadRequest(new LoginResponse { Error = $"{string.Join(",", errors)}", ErrorCode = "A01" });
                }
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                var response = await service.RefreshTokenAsync(request, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);
                if (!response.Success) return response.ErrorCode switch
                {
                    "REFRESH_FAILED"  => Unauthorized(response),
                    "REFRESH_ERROR" => StatusCode(500, response),
                    "INVALID_REFRESH_TOKEN" => StatusCode(403, response),
                    _ => BadRequest(response)
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        /// <summary>
        /// Delete user session from the system
        /// <paramref name="latitude"/>
        /// <paramref name="longitude"/>
        /// </summary>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="403">HTTP Forbidden, Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpDelete]
        [Route("logout")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async  Task<IActionResult> Logout([FromQuery] string latitude, [FromQuery] string longitude)
        {
            try
            {
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                string session = findClaimHelper.FindClaim(HttpContext, "sid");
                var response = await service.LogoutAsync(session, latitude, longitude, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);
                if (!response.Success) return response.ErrorCode switch
                {
                    "REFRESH_FAILED" => Unauthorized(response),
                    "LOGOUT_ERROR" or "INVALID_PARAMETERS" => StatusCode(500, response),
                    "NO_ACTIVE_SESSIONS" => StatusCode(403, response),
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
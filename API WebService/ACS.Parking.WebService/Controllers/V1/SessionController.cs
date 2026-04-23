using ACS.Helper.V1;
using ACS.Models.Response;
using ACS.Models.Response.V1.AuthenticationService.Account;
using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Models.Response.V1.ParkingService.Payment;
using ACS.Models.Response.V1.ParkingService.Session;
using ACS.Parking.WebService.Models.Request.V1;
using ACS.Parking.WebService.Services.V1.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;

namespace ACS.Parking.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class SessionController(ISessionService service, FindClaimHelper findClaimHelper) : BaseController
    {

        /// <summary>
        /// Create entry session
        /// </summary>
        /// <param name="request"></param>
        /// <param name="project"></param>
        /// <param name="projectArea"></param>
        /// <param name="zone"></param>
        /// <param name="accessPoint"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK. Returned when the request is processed successfully.</response>
        /// <response code="400">HTTP Bad Request. Returned when the request data is invalid, malformed, or missing required fields.</response>
        /// <response code="402">HTTP Payment Required. Returned when payment is required or the account balance/plan is insufficient.</response>
        /// <response code="403">HTTP Forbidden. Returned when the authenticated user does not have permission to perform this action.</response>
        /// <response code="404">HTTP Not Found. Returned when the requested resource does not exist.</response>
        /// <response code="409">HTTP Conflict. Returned when the request conflicts with the current state of the resource (e.g., duplicate parking session, already checked-in/out).</response>
        /// <response code="500">HTTP Internal Server Error. Returned when an unexpected server-side error occurs.</response>
        [HttpPost]
        [Route("create")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(EntrySessionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EntrySessionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EntrySessionResponse), StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(typeof(EntrySessionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(EntrySessionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(EntrySessionResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "entry.create"*/)]
        public async Task<IActionResult> Create([FromBody] CreateSessionRequest request,
            [FromHeader(Name = "project")][Required] string project = "2",
            [FromHeader(Name = "area")][Required] string projectArea = "3",
            [FromHeader(Name = "zone")][Required] string zone = "1",
            [FromHeader(Name = "access-point")][Required] string accessPoint = "1")
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
                    if (errors.Count != 0) return BadRequest(new LoginResponse { Error = $"{string.Join(",", errors)}", ErrorCode = "A01" });
                }
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string session = findClaimHelper.FindClaim(HttpContext, "sid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                var response = await service.CreateEntrySessionAsync(workspace, project, projectArea, zone,
                    accessPoint, session, user, request, ipAddress, null, serAgent, requestId, 
                    HttpContext.RequestAborted);
                if (!response.Success) return response.ErrorCode switch
                {
                    "WORKSPACE_EXPIRED" => StatusCode(402, response),
                    "ZONE_FULL" => StatusCode(409, response),
                    "ACCESS_POINT_INACTIVE" or "ZONE_INACTIVE"  => StatusCode(403, response),
                    "INVALID_WORKSPACE" or "INVALID_PROJECT" or "INVALID_ZONE" => StatusCode(404, response),
                    _ => StatusCode(400, response)
                };
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="project"></param>
        /// <param name="projectArea"></param>
        /// <param name="zone"></param>
        /// <param name="accessPoint"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK. Returned when the request is processed successfully.</response>
        /// <response code="400">HTTP Bad Request. Returned when the request data is invalid, malformed, or missing required fields.</response>
        /// <response code="402">HTTP Payment Required. Returned when payment is required or the account balance/plan is insufficient.</response>
        /// <response code="403">HTTP Forbidden. Returned when the authenticated user does not have permission to perform this action.</response>
        /// <response code="404">HTTP Not Found. Returned when the requested resource does not exist.</response>
        /// <response code="409">HTTP Conflict. Returned when the request conflicts with the current state of the resource (e.g., duplicate parking session, already checked-in/out).</response>
        /// <response code="500">HTTP Internal Server Error. Returned when an unexpected server-side error occurs.</response>
        [HttpGet]
        [Route("validate/{session}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "exit.valiate"*/)]
        public async Task<IActionResult> ValidateBySessionId(
            [FromRoute][Required(ErrorMessage = "session ID parameter is required")] string session,
            [FromQuery] string latitude, [FromQuery] string longitude,
            [FromHeader(Name = "project")][Required] string project = "2",
            [FromHeader(Name = "area")][Required] string projectArea = "3",
            [FromHeader(Name = "zone")][Required] string zone = "1",
            [FromHeader(Name = "access-point")][Required] string accessPoint = "1")
        {
            try
            {
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                if (!double.TryParse(latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude = 0;
                if (!double.TryParse(longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;

                int? sessionId = null;
                string? sessionCode = null;
                if (int.TryParse(session, out int parsedSessionId)) sessionId = parsedSessionId;
                else sessionCode = session;


                var response = await service.ValidateSessionBySessionIdAsync(workspace, project, projectArea, zone, accessPoint, sessionCode?? "",
                    sessionId?.ToString() ?? "", "", "", "", "", "", user, _latitude, _longitude, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);
                if (!response.Success) return response.ErrorCode switch
                {
                    "WORKSPACE_EXPIRED" => StatusCode(402, response),
                    "ZONE_FULL" => StatusCode(409, response),
                    "SESSION_VALIDATION_FAILED" or "INVALID_PROJECT" or "INVALID_ZONE" => StatusCode(404, response),
                    "VALIDATION_ERROR" or "VALIDATION_SYSTEM_ERROR" or "TARIFF_CALCULATION_FAILED" or "TARIFF_CALCULATION_ERROR" => StatusCode(500, response),
                    _ => StatusCode(400, response)
                };
                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="plateCode"></param>
        /// <param name="plateNumber"></param>
        /// <param name="country"></param>
        /// <param name="state"></param>
        /// <param name="category"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="project"></param>
        /// <param name="projectArea"></param>
        /// <param name="zone"></param>
        /// <param name="accessPoint"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK. Returned when the request is processed successfully.</response>
        /// <response code="400">HTTP Bad Request. Returned when the request data is invalid, malformed, or missing required fields.</response>
        /// <response code="402">HTTP Payment Required. Returned when payment is required or the account balance/plan is insufficient.</response>
        /// <response code="403">HTTP Forbidden. Returned when the authenticated user does not have permission to perform this action.</response>
        /// <response code="404">HTTP Not Found. Returned when the requested resource does not exist.</response>
        /// <response code="409">HTTP Conflict. Returned when the request conflicts with the current state of the resource (e.g., duplicate parking session, already checked-in/out).</response>
        /// <response code="500">HTTP Internal Server Error. Returned when an unexpected server-side error occurs.</response>
        [HttpGet]
        [Route("validate")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidateSessionResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "exit.valiate"*/)]
        public async Task<IActionResult> Validate(
            [FromQuery] string? plateCode,
            [FromQuery][Required(ErrorMessage = "Plate Number parameter is required")] string plateNumber = "1249",
            [FromQuery][Required(ErrorMessage = "Country ID parameter is required")] int country= 89,
            [FromQuery][Required(ErrorMessage = "State ID parameter is required")] int state =9,
            [FromQuery][Required(ErrorMessage = "Category ID parameter is required")] int category =1,
            [FromQuery] string latitude = "0",
            [FromQuery] string longitude = "0",
            [FromHeader(Name = "project")][Required] string project = "2",
            [FromHeader(Name = "area")][Required] string projectArea = "3",
            [FromHeader(Name = "zone")][Required] string zone = "1",
            [FromHeader(Name = "access-point")][Required] string accessPoint = "1")

        {
            try
            {
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                if (!double.TryParse(latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude = 0;
                if (!double.TryParse(longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;

            
                var response = await service.ValidateSessionBySessionIdAsync(workspace, project, projectArea, zone, accessPoint,"", "", plateCode ?? "", plateNumber, country.ToString(),
                    state.ToString(), category.ToString(),  user, _latitude, _longitude, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);
                if (!response.Success) return response.ErrorCode switch
                {
                    "WORKSPACE_EXPIRED" => StatusCode(402, response),
                    "ZONE_FULL" => StatusCode(409, response),
                    "SESSION_VALIDATION_FAILED" or "INVALID_PROJECT" or "INVALID_ZONE" => StatusCode(404, response),
                    "VALIDATION_ERROR" or "VALIDATION_SYSTEM_ERROR" or "TARIFF_CALCULATION_FAILED" or "TARIFF_CALCULATION_ERROR" => StatusCode(500, response),
                    _ => StatusCode(400, response)
                };
                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="project"></param>
        /// <param name="projectArea"></param>
        /// <param name="zone"></param>
        /// <param name="accessPoint"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK. Returned when the request is processed successfully.</response>
        /// <response code="400">HTTP Bad Request. Returned when the request data is invalid, malformed, or missing required fields.</response>
        /// <response code="402">HTTP Payment Required. Returned when payment is required or the account balance/plan is insufficient.</response>
        /// <response code="403">HTTP Forbidden. Returned when the authenticated user does not have permission to perform this action.</response>
        /// <response code="404">HTTP Not Found. Returned when the requested resource does not exist.</response>
        /// <response code="409">HTTP Conflict. Returned when the request conflicts with the current state of the resource (e.g., duplicate parking session, already checked-in/out).</response>
        /// <response code="500">HTTP Internal Server Error. Returned when an unexpected server-side error occurs.</response>
        [HttpPost]
        [Route("pay/{session}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ProcessPaymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProcessPaymentResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProcessPaymentResponse), StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(typeof(ProcessPaymentResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProcessPaymentResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProcessPaymentResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "exit.valiate"*/)]
        public async Task<IActionResult> Pay([FromBody] PaymentSessionRequest request,
             [FromRoute][Required(ErrorMessage = "session ID parameter is required")] int session,
            [FromHeader(Name = "project")][Required] string project = "2",
            [FromHeader(Name = "area")][Required] string projectArea = "3",
            [FromHeader(Name = "zone")][Required] string zone = "1",
            [FromHeader(Name = "access-point")][Required] string accessPoint = "1")
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
                    if (errors.Count != 0) return BadRequest(new LoginResponse { Error = $"{string.Join(",", errors)}", ErrorCode = "A01" });
                }
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string usersSession = findClaimHelper.FindClaim(HttpContext, "sid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                var response = await service.PaymentSessionAsync(workspace, project, projectArea, zone, accessPoint, usersSession,
                    user, request, session, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);
                if (!response.Success) return response.ErrorCode switch
                {
                    "WORKSPACE_EXPIRED" => StatusCode(402, response),
                    "ALREADY_PAID" => StatusCode(409, response),
                    "ACCESS_POINT_INACTIVE" or "ZONE_INACTIVE" => StatusCode(403, response),
                    "INVALID_WORKSPACE" or "INVALID_PROJECT" or "INVALID_ZONE" => StatusCode(404, response),
                    "TARIFF_CALCULATION_ERROR" or "TAX_CALCULATION_ERROR" or "INVOICE_GENERATION_ERROR" => StatusCode(500, response),
                    _ => StatusCode(400, response)
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

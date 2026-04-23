using ACS.Helper.V1;
using Asp.Versioning;
using ACS.Models.Response;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using ACS.Models.Response.V1.ParkingService.Session;
using ACS.Parking.WebService.Services.V1.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ACS.Models.Response.V1.AuthenticationService.Account; 

namespace ACS.Parking.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AccessPointController(IAccessPointSessionService service, FindClaimHelper findClaimHelper) : BaseController
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="projectArea"></param>
        /// <param name="zone"></param>
        /// <param name="accessPoint"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK. Returned when the request is processed successfully.</response>
        /// <response code="400">HTTP Bad Request. Returned when the request data is invalid, malformed, or missing required fields.</response>
        /// <response code="403">HTTP Forbidden. Returned when the authenticated user does not have permission to perform this action.</response>
        /// <response code="404">HTTP Not Found. Returned when the requested resource does not exist.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when an unexpected server-side error occurs.</response>
        [HttpGet]
        [Route("sesions")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(SessionsResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "accesspoint.sessions"*/)]
        public async Task<IActionResult> GetSessions(
        [FromQuery(Name = "project")][Required] int project = 2,
        [FromQuery(Name = "project-area")][Required] int projectArea = 3,
        [FromQuery(Name = "zone")][Required] int zone = 1,
        [FromQuery(Name = "access-point")][Required] int accessPoint = 1,
        [FromQuery(Name = "skip")][Required] int skip = 0,
        [FromQuery(Name = "take")][Required] int take = 20,
        [FromQuery(Name = "status")][Required] string? status = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
                    if (errors.Count != 0) return BadRequest(new LoginResponse { Error = $"{string.Join(",", errors)}", ErrorCode = "S01" });
                }
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                if (!int.TryParse(workspace, NumberStyles.Float, CultureInfo.InvariantCulture, out int _workspace)) _workspace = 0;
                var response = await service.GetParkingSessionsByAccessPointAsync(_workspace, project, projectArea, zone, accessPoint, skip, take, status, cancellationToken: HttpContext.RequestAborted);
                if (!response.Success) return StatusCode(400, response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}

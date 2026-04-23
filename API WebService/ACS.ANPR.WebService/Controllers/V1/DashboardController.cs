using ACS.Helper.V1;
using Asp.Versioning;
using ACS.Models.Response;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using ACS.ANPR.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.ANPRService.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ACS.ANPR.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class DashboardController(IDashboardService service, FindClaimHelper findClaimHelper) : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Dashboard data retrieved successfully.</response>
        /// <response code="401">Missing or invalid authentication token.</response>
        /// <response code="500">Unexpected server-side error.</response>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AnprDashboardResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetDashboardAsync()
        {
            try
            {
                string requestId = GetRequestId();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");

                if (string.IsNullOrEmpty(workspace) || !int.TryParse(workspace, out _))
                {
                    return Unauthorized(new BaseResponses(
                        Success: false,
                        Message: "Invalid or missing workspace identifier",
                        ErrorCode: "INVALID_WORKSPACE",
                        RequestId: requestId));
                }

                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                var response = await service.GetDashboardAsync(workspace, user, requestId, HttpContext.RequestAborted);
                if (!response.Success) return StatusCode(500, response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves dashboard information for the authenticated user's workspace.
        /// </summary>
        /// <param name="projectId">Project identifier. Must be a positive integer.</param>
        /// <param name="viewType">Aggregation view: daily | weekly | monthly.</param>
        /// <param name="days">Number of days for the daily trend window (1–365).</param>
        /// <response code="200">Dashboard data retrieved successfully.</response>
        /// <response code="401">Missing or invalid authentication token.</response>
        /// <response code="500">Unexpected server-side error.</response>
        [HttpGet("{project_id:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AnprDashboardProjectResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetDashboardByProjectAsync(
            [FromRoute(Name = "project_id")][Range(1, int.MaxValue, ErrorMessage = "project_id must be a positive integer")] int projectId,
            [FromQuery][RegularExpression("^(daily|weekly|monthly)$",  ErrorMessage = "viewType must be daily, weekly, or monthly")] string? viewType = "daily",
            [FromQuery][Range(1, 365, ErrorMessage = "days must be between 1 and 365")]int? days = 30)
        {
            try
            {
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");

                if (string.IsNullOrEmpty(workspace) || !int.TryParse(workspace, out _))
                {
                    return Unauthorized(new BaseResponses(
                        Success: false,
                        Message: "Invalid or missing workspace identifier",
                        ErrorCode: "INVALID_WORKSPACE",
                        RequestId: requestId));
                }

                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                var response = await service.GetDashboardByProjectAsync(
                    workspaceId: workspace,
                    projectId: projectId,
                    viewType: viewType ?? "daily",
                    days: days ?? 30,
                    userId: user,
                    requestId: requestId,
                    cancellationToken: HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}

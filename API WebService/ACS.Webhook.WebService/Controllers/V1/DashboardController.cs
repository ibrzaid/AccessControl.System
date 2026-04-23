using ACS.Helper.V1;
using Asp.Versioning;
using ACS.Models.Response;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.Webhook.WebService.Services.V1.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ACS.Models.Response.V1.WebhooksService.Dashboard;

namespace ACS.Webhook.WebService.Controllers.V1
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
        [ProducesResponseType(typeof(WebhooksDashboardResponse), StatusCodes.Status200OK)]
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
    }
}

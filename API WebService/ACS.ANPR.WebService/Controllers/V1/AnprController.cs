using ACS.Helper.V1;
using Asp.Versioning;
using ACS.Models.Response;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ACS.Models.Request.V1.ANPRService;
using Microsoft.AspNetCore.Authorization;
using ACS.Models.Response.V1.ANPRService.Anpr;
using ACS.ANPR.WebService.Services.V1.Interfaces;

namespace ACS.ANPR.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AnprController(IAnprService service, FindClaimHelper findClaimHelper) : BaseController
    {

        private static AnprInsertResponse Fail(string? requestId, string code, string msg) =>
            new(Success: false, Message: msg, ErrorCode: code,
                RequestId: requestId ?? "", Errors: null, Warnings: null, Data: null, Metadata: null);

        [HttpPost]
        [Route("detection")]
        [MapToApiVersion("1.0")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AnprInsertResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AnprInsertResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = "Basic")]
        public async Task<IActionResult> SubmitDetectionAsync([FromForm] AnprInsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values
                        .SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                        .ToList();
                    return BadRequest(Fail(null, "VALIDATION_ERROR", string.Join(", ", errs)));
                }

                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                string userAgent = Request.Headers.UserAgent.ToString();
                string serialNumber = findClaimHelper.FindClaim(HttpContext, ClaimTypes.Name);

                string workspaceId = findClaimHelper.FindClaim(HttpContext, "wid");
                string hardwareIdStr = findClaimHelper.FindClaim(HttpContext, "hardware_id");
                string accessPointStr = findClaimHelper.FindClaim(HttpContext, "access_point_id");
                string zoneIdStr = findClaimHelper.FindClaim(HttpContext, "zone_id");
                string areaIdStr = findClaimHelper.FindClaim(HttpContext, "project_area_id");
                string projectIdStr = findClaimHelper.FindClaim(HttpContext, "project_id");
                string laneStr = findClaimHelper.FindClaim(HttpContext, "lane_number");
                string apPrefix = findClaimHelper.FindClaim(HttpContext, "access_point_prefix");

                if (string.IsNullOrEmpty(workspaceId)   ||
                    string.IsNullOrEmpty(hardwareIdStr)  ||
                    !int.TryParse(workspaceId, out _) ||
                    !int.TryParse(hardwareIdStr, out int hardwareId)    ||
                    !int.TryParse(accessPointStr, out int accessPointId) ||
                    !int.TryParse(zoneIdStr, out int zoneId)        ||
                    !int.TryParse(areaIdStr, out int projectAreaId) ||
                    !int.TryParse(projectIdStr, out int projectId))
                    return Unauthorized(Fail(requestId, "INVALID_CLAIMS",
                        "Camera claims are incomplete. Re-authenticate."));

                var response = await service.InsertAync(request, workspaceId, projectId, projectAreaId, zoneId, accessPointId, hardwareId,
                    ipAddress, userAgent, requestId, HttpContext.RequestAborted);
                
                if (!response.Success) return response.ErrorCode switch
                {
                    "INVALID_WORKSPACE" or "UNAUTHORIZED" => Unauthorized(response),
                    "LICENSE_VALIDATION_FAILED" => StatusCode(402, response),
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

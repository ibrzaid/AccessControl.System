using ACS.Helper.V1;
using Asp.Versioning;
using ACS.Models.Response;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ACS.Notifications.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.NotificationsService.UserNotifications;

namespace ACS.Notifications.WebService.Controllers.V1
{
    /// <summary>
    /// User-facing notifications endpoints (paged list + delete).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class NotificationsController(IDashboardService service, FindClaimHelper findClaimHelper) : BaseController
    {
        private static decimal ParseCoord(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return 0m;
            return decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : 0m;
        }

        /// <summary>
        /// Paginated list of the current user's notifications. Supports a
        /// load-more flow via `limit` / `offset`, and an `unreadOnly` filter.
        /// </summary>
        /// <param name="limit">Max items to return (1-100, default 20).</param>
        /// <param name="offset">Items to skip (default 0).</param>
        /// <param name="unreadOnly">If true, only unread notifications.</param>
        /// <param name="latitude">Optional caller latitude (recorded in audit log).</param>
        /// <param name="longitude">Optional caller longitude (recorded in audit log).</param>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(UserNotificationsListResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserNotificationsAsync(
            [FromQuery] int limit = 20,
            [FromQuery] int offset = 0,
            [FromQuery] bool unreadOnly = false,
            [FromQuery] string? latitude = null,
            [FromQuery] string? longitude = null)
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

                if (limit < 1) limit = 1;
                if (limit > 100) limit = 100;
                if (offset < 0) offset = 0;

                string? ip  = HttpContext.Connection.RemoteIpAddress?.ToString();
                string? ua  = Request.Headers.UserAgent.ToString();
                string? di  = Request.Headers["X-Device-Info"].ToString();
                if (string.IsNullOrWhiteSpace(di)) di = null;

                var response = await service.GetUserNotificationsAsync(
                    workspace, user, limit, offset, unreadOnly,
                    ip, ua, di, requestId,
                    ParseCoord(latitude), ParseCoord(longitude),
                    HttpContext.RequestAborted);

                if (!response.Success) return StatusCode(500, response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Delete one of the calling user's notifications.
        /// </summary>
        [HttpDelete("{id:long}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(DeleteUserNotificationResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteUserNotificationAsync(
            [FromRoute] long id,
            [FromQuery] string? latitude = null,
            [FromQuery] string? longitude = null)
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

                string? ip  = HttpContext.Connection.RemoteIpAddress?.ToString();
                string? ua  = Request.Headers.UserAgent.ToString();
                string? di  = Request.Headers["X-Device-Info"].ToString();
                if (string.IsNullOrWhiteSpace(di)) di = null;

                var response = await service.DeleteUserNotificationAsync(
                    workspace, user, id,
                    ip, ua, di, requestId,
                    ParseCoord(latitude), ParseCoord(longitude),
                    HttpContext.RequestAborted);

                if (!response.Success)
                {
                    if (response.ErrorCode == "NOT_FOUND") return NotFound(response);
                    return StatusCode(500, response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}

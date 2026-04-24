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
    public class NotificationsController(IUserNotificationsService service, FindClaimHelper findClaimHelper) : BaseController
    {
        private static decimal ParseCoord(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return 0m;
            return decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : 0m;
        }

        /// <summary>
        /// Paginated list of the current user's notifications. Supports a
        /// load-more flow via `limit` / `offset`, and a 3-way `filterStatus`
        /// filter ("all" | "unread" | "read").
        /// </summary>
        /// <param name="limit">Max items to return (1-100, default 20).</param>
        /// <param name="offset">Items to skip (default 0).</param>
        /// <param name="filterStatus">"all" (default), "unread", or "read".</param>
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
            [FromQuery] string filterStatus = "all",
            [FromQuery] string? search = null,
            [FromQuery] string? priority = null,
            [FromQuery] string? type = null,
            [FromQuery] string? since = null,
            [FromQuery] string? until = null,
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
                    workspace, user, limit, offset, filterStatus, search,
                    priority, type, since, until,
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

        /// <summary>
        /// Bulk-delete a set of notifications owned by the calling user. The
        /// SQL function caps the array at 500 ids per call.
        /// </summary>
        [HttpPost("bulk-delete")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BulkDeleteUserNotificationsResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> BulkDeleteUserNotificationsAsync(
            [FromBody] BulkDeleteUserNotificationsRequest body,
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

                if (body is null || body.Ids is null || body.Ids.Length == 0)
                {
                    return BadRequest(new BaseResponses(
                        Success: false,
                        Message: "ids array is required",
                        ErrorCode: "EMPTY_INPUT",
                        RequestId: requestId));
                }

                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                string? ip  = HttpContext.Connection.RemoteIpAddress?.ToString();
                string? ua  = Request.Headers.UserAgent.ToString();
                string? di  = Request.Headers["X-Device-Info"].ToString();
                if (string.IsNullOrWhiteSpace(di)) di = null;

                var response = await service.BulkDeleteUserNotificationsAsync(
                    workspace, user, body.Ids,
                    ip, ua, di, requestId,
                    ParseCoord(latitude), ParseCoord(longitude),
                    HttpContext.RequestAborted);

                if (!response.Success)
                {
                    if (response.ErrorCode == "EMPTY_INPUT" || response.ErrorCode == "BATCH_TOO_LARGE")
                        return BadRequest(response);
                    return StatusCode(500, response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Mark one notification owned by the calling user as read. Idempotent
        /// (a no-op if it was already read). Emits a SignalR event so other
        /// open tabs/devices for the same user reconcile their unread badge.
        /// </summary>
        [HttpPatch("{id:long}/read")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(MarkUserNotificationReadResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> MarkUserNotificationReadAsync(
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

                var response = await service.MarkUserNotificationReadAsync(
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

        /// <summary>
        /// Mark every unread notification belonging to the calling user as
        /// read in a single round-trip.
        /// </summary>
        [HttpPost("read-all")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(MarkAllUserNotificationsReadResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> MarkAllUserNotificationsReadAsync(
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

                var response = await service.MarkAllUserNotificationsReadAsync(
                    workspace, user,
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
        /// Phase G — read the calling user's notification preferences. The
        /// response also carries the active priority/type catalogues so the
        /// preferences UI can render labels in the user's locale without an
        /// extra round trip.
        /// </summary>
        [HttpGet("preferences")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(UserNotificationPreferencesResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserNotificationPreferencesAsync()
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

                var response = await service.GetUserNotificationPreferencesAsync(
                    workspace, user, requestId, HttpContext.RequestAborted);

                if (!response.Success) return StatusCode(500, response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Phase G — replace the calling user's notification mute lists. Send
        /// an empty array to clear all mutes for that dimension. Returns the
        /// fresh state (including the catalogues) so the UI can re-render
        /// without a follow-up GET.
        /// </summary>
        [HttpPut("preferences")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(UserNotificationPreferencesResponse), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserNotificationPreferencesAsync(
            [FromBody] UpdateUserNotificationPreferencesRequest body)
        {
            try
            {
                string requestId = GetRequestId();
                if (body is null)
                {
                    return BadRequest(new BaseResponses(
                        Success: false,
                        Message: "Request body is required",
                        ErrorCode: "EMPTY_BODY",
                        RequestId: requestId));
                }

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

                var response = await service.UpdateUserNotificationPreferencesAsync(
                    workspace, user,
                    body.MutedPriorities ?? Array.Empty<string>(),
                    body.MutedTypes      ?? Array.Empty<string>(),
                    requestId, HttpContext.RequestAborted);

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

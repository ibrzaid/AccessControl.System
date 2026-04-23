using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Helper.V1;
using ACS.Models.Request.V1.AuthenticationService.Avatar;
using ACS.Models.Request.V1.AuthenticationService.ManageUser;
using ACS.Models.Response;
using ACS.Models.Response.V1.AuthenticationService.Avatar;
using ACS.Models.Response.V1.AuthenticationService.UserLogs;
using ACS.Models.Response.V1.AuthenticationService.UserManagement;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;

namespace ACS.Authentication.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class UsersController(IUserManagementService service, IUserLogsService userLogsService, FindClaimHelper findClaimHelper) : BaseController
    {

        /// <summary>
        /// List users in the caller's workspace.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="search"></param>
        /// <param name="statusId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetUsersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUsersAsync(
            [FromQuery] int userId = 0,
            [FromQuery] string? search = null,
            [FromQuery] int statusId = 0,
            [FromQuery] int limit = 50,
            [FromQuery] int offset = 0)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                var response = await service.GetUsersAsync(
                    int.Parse(workspace), int.Parse(user), userId, search, statusId, limit, offset,
                    HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Get full detail for a single user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("{userId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] int userId)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                var response = await service.GetUserByIdAsync(
                    userId, int.Parse(workspace), HttpContext.RequestAborted);

                if (!response.Success) return NotFound(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Create a new user.
        /// Validates workspace license and available seats first.
        /// Password hashed with bcrypt. Timezone/language inherit from workspace if not supplied.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="201">Created</response>
        /// <response code="400">Validation error / username or email taken / password too short</response>
        /// <response code="402">License check failed / seats exhausted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new CreateUserResponse
                    {
                        Success   = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Error   = string.Join(", ", errors)
                    });
                }

                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();

                var response = await service.CreateUserAsync(
                    int.Parse(user), int.Parse(workspace), request, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                if (!response.Success)
                    return response.ErrorCode switch
                    {
                        "LICENSE_CHECK_FAILED" => StatusCode(402, response),
                        "USERNAME_TAKEN"
                        or "EMAIL_TAKEN"
                        or "PASSWORD_TOO_SHORT" => BadRequest(response),
                        _ => BadRequest(response)
                    };

                return StatusCode(201, response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        ///  Update user profile.
        ///  Only fields provided in the request body are changed (null = no change).
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Validation error</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPatch]
        [Route("{userId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserAsync(
            [FromRoute] int userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new UserOperationResponse
                    {
                        Success   = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Error   = string.Join(", ", errors)
                    });
                }
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();

                var response = await service.UpdateUserAsync(
                    int.Parse(user), userId, int.Parse(workspace), request, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                if (!response.Success)
                    return response.ErrorCode == "USER_NOT_FOUND"
                        ? NotFound(response)
                        : BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Change user status: ACTIVE | INACTIVE | SUSPENDED | LOCKED.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Invalid status / validation error</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPatch]
        [Route("{userId:int}/status")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SetUserStatusAsync(
            [FromRoute] int userId, [FromBody] SetUserStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new UserOperationResponse
                    {
                        Success   = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Error   = string.Join(", ", errors)
                    });
                }

                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();

                var response = await service.SetUserStatusAsync(
                     int.Parse(user), userId, int.Parse(workspace), request, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                if (!response.Success)
                    return response.ErrorCode switch
                    {
                        "USER_NOT_FOUND" => NotFound(response),
                        "INVALID_STATUS" => BadRequest(response),
                        _ => BadRequest(response)
                    };

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Soft-delete a user.
        /// Sets status to INACTIVE, revokes all access rules,
        /// and decrements the workspace seat counter.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Validation error / self-delete attempt</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("{userId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteUserAsync(
            [FromRoute] int userId, [FromBody] DeleteUserRequest? request = null)
        {
            try
            {

                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();

                var response = await service.DeleteUserAsync(
                    int.Parse(user), userId, int.Parse(workspace),
                    request ?? new DeleteUserRequest(), ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                if (!response.Success)
                    return response.ErrorCode switch
                    {
                        "USER_NOT_FOUND" => NotFound(response),
                        "SELF_DELETE_FORBIDDEN" => BadRequest(response),
                        _ => BadRequest(response)
                    };

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Admin password reset. No old-password required. Minimum 8 characters.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Validation / password too short</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("{userId:int}/reset-password")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ResetPasswordAsync(
            [FromRoute] int userId, [FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new UserOperationResponse
                    {
                        Success   = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Error   = string.Join(", ", errors)
                    });
                }

                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();

                var response = await service.ResetPasswordAsync(
                    int.Parse(user), userId, int.Parse(workspace), request, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                if (!response.Success)
                    return response.ErrorCode switch
                    {
                        "USER_NOT_FOUND" => NotFound(response),
                        "PASSWORD_TOO_SHORT" => BadRequest(response),
                        _ => BadRequest(response)
                    };

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Get paginated activity / audit log for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("{userId:int}/activity")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserActivityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserActivityAsync(
            [FromRoute] int userId,
            [FromQuery] int limit = 50,
            [FromQuery] int offset = 0)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string caller   = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                var response = await service.GetUserActivityAsync(
                    userId, int.Parse(workspace), int.Parse(caller), limit, offset, HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Assign a role to a user. Idempotent — safe to call if already assigned.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Validation / role or user not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("{userId:int}/roles")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AssignRoleAsync(
            [FromRoute] int userId, [FromBody] AssignRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new UserOperationResponse
                    {
                        Success   = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Error   = string.Join(", ", errors)
                    });
                }

                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();

                var response = await service.AssignRoleAsync(
                    int.Parse(user), userId, int.Parse(workspace), request, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }



        /// <summary>
        /// Remove a role from a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Assignment not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("{userId:int}/roles/{roleId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RemoveRoleAsync(
            [FromRoute] int userId, [FromRoute] int roleId)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                var response = await service.RemoveRoleAsync(
                    int.Parse(user), userId, int.Parse(workspace), roleId, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Update Avatar
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Assignment not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("avatar")]
        [MapToApiVersion("1.0")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UpdateAvatarResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UpdateAvatarResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UpdateAvatarResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateAvatar( [FromForm] UpdateAvatarRequest avatar)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new UserOperationResponse
                    {
                        Success   = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Error   = string.Join(", ", errors)
                    });
                }
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string serAgent = Request.Headers.UserAgent.ToString();
                if (!double.TryParse(avatar.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude  = 0;
                if (!double.TryParse(avatar.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;

                var response = await service.UpdateAvatar(
                    int.Parse(workspace), 
                    int.Parse(user),  
                    avatar.UserId, 
                    avatar.File, 
                    ipAddress,
                    null, 
                    serAgent, 
                    requestId, 
                    _latitude, 
                    _longitude,  
                    HttpContext.RequestAborted);
                if (response == null || !response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Get user logs with optional filtering by entity type and action, within a specified date range.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="orderBy"></param>
        /// <param name="userId"></param>
        /// <param name="entityType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Assignment not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("audit")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserLogsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserLogsResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponses), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserLogsAsync(
            [FromQuery(Name = "from_date")][Required(ErrorMessage = "From Date is required")] DateTime fromDate,
            [FromQuery(Name = "to_date")][Required(ErrorMessage = "From Date is required")] DateTime toDate,
            [FromQuery(Name = "limit")][Range(1, int.MaxValue)][Required(ErrorMessage = "Limit is required")] int limit = 50,
            [FromQuery(Name = "page")][Range(1, 500)][Required(ErrorMessage = "Page is required")] int page = 1,
            [FromQuery][RegularExpression("^(DESC|ASC)$", ErrorMessage = "viewType must be DESC or ASC")] string? orderBy = "DESC",
            [FromQuery(Name = "user_id")][Range(1, int.MaxValue)] int? userId = null,
            [FromQuery(Name = "entity_type")][StringLength(20)] string? entityType = null,
            [FromQuery(Name = "action")][StringLength(20)] string? action = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new UserLogsResponse
                    {
                        Success   = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Error   = string.Join(", ", errors)
                    });
                }
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string requestId = GetRequestId();
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                var response = await userLogsService.GetUserLogsAsync(
                    int.Parse(workspace), 
                    int.Parse(user), 
                    userId, 
                    action, 
                    entityType, 
                    fromDate, 
                    toDate, 
                    requestId, 
                    limit, 
                    page,
                    orderBy,
                    HttpContext.RequestAborted);
                if (response == null || !response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }



       

    }
}

using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Helper.V1;
using ACS.Models.Request.V1.AuthenticationService.AccessException;
using ACS.Models.Request.V1.AuthenticationService.AccessUser;
using ACS.Models.Request.V1.AuthenticationService.ManageUser;
using ACS.Models.Response;
using ACS.Models.Response.V1.AuthenticationService.AccessLevel;
using ACS.Models.Response.V1.AuthenticationService.AccessRule;
using ACS.Models.Response.V1.AuthenticationService.AccessScope;
using ACS.Models.Response.V1.AuthenticationService.Avatar;
using ACS.Models.Response.V1.AuthenticationService.UserAccess;
using ACS.Models.Response.V1.AuthenticationService.UserAccessException;
using ACS.Models.Response.V1.AuthenticationService.UserManagement;
using ACS.Service;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace ACS.Authentication.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AccessController(IUserManagementService userManagementService,  IUserAccessService userAccessService,  IUserAccessRuleService userAccessRuleService, IUserAccessExceptionService userAccessExceptionService, IUserAccessScopeService userAccessScopeService, IUserAccessLevelService userAccessLevelService, FindClaimHelper findClaimHelper) : BaseController
    {

        /// <summary>
        /// Get access rules. userId=0 → all users in workspace.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="includeBreadcrumbs"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("rules")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetAccessRulesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAccessRulesAsync(
             [FromQuery] int userId = 0,
             [FromQuery] bool includeBreadcrumbs = false)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                var response = await userAccessRuleService.GetAccessRulesAsync(
                    int.Parse(workspace), 
                    int.Parse(user),  
                    userId, 
                    includeBreadcrumbs, 
                    HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Get access exceptions. userId=0 → all users.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("exceptions")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetAccessExceptionsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAccessExceptionsAsync([FromQuery] int userId = 0)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                var response = await userAccessExceptionService.GetAccessExceptionsAsync(
                     int.Parse(workspace), userId, HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// >Get access audit history. userId=0 → all users.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("history")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetAccessHistoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAccessHistoryAsync(
            [FromQuery] int userId = 0,
            [FromQuery] int limit = 100,
            [FromQuery] int offset = 0)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string caller   = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                var response = await userAccessService.GetAccessHistoryAsync(
                     int.Parse(workspace), userId, int.Parse(caller), limit, offset, HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// All scope types for dropdowns.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("scopes")]
        [MapToApiVersion("1.0")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GetAccessScopesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccessScopesAsync()
        {
            try
            {
                var response = await userAccessScopeService.GetAccessScopesAsync(HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// All access levels for dropdowns.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("levels")]
        [AllowAnonymous]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetAccessLevelsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccessLevelsAsync()
        {
            try
            {
                var response = await userAccessLevelService.GetAccessLevelsAsync(HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Grant or upsert user access at any scope.
        /// Response Action: inserted | upgraded | reactivated | unchanged.
        /// Never downgrades an existing higher-level rule.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Validation error</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("rules")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GrantAccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GrantAccessResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GrantAccessAsync([FromBody] GrantAccessRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new GrantAccessResponse
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

                var response = await userAccessService.GrantAccessAsync(
                    int.Parse(user), int.Parse(workspace), request, ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Revoke (soft-delete) an access rule by its ID
        /// </summary>
        /// <param name="userAccessId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="404">Rule not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("rules/{userAccessId:long}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RevokeAccessAsync(
            [FromRoute] long userAccessId, [FromBody] RevokeAccessRequest? request = null)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();

                var response = await userAccessService.RevokeAccessAsync(
                     int.Parse(user), userAccessId, int.Parse(workspace),
                    request ?? new RevokeAccessRequest(), ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                if (!response.Success)
                    return response.ErrorCode == "RULE_NOT_FOUND"
                        ? NotFound(response)
                        : BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Add a deny or allow-override exception for a specific sub-resource.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="201">Created</response>
        /// <response code="400">Validation error</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("exceptions")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddAccessExceptionAsync([FromBody] AddExceptionRequest request)
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

                var response = await userAccessExceptionService.AddAccessExceptionAsync(
                    int.Parse(user), int.Parse(workspace), request, ipAddress, null, serAgent, requestId,  HttpContext.RequestAborted);

                return StatusCode(201, response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Delete an access exception by its ID.
        /// </summary>
        /// <param name="exceptionId"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="404">Exception not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("exceptions/{exceptionId:long}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserOperationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteAccessExceptionAsync([FromRoute] long exceptionId)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();


                var response = await userAccessExceptionService.DeleteAccessExceptionAsync(
                     int.Parse(user), exceptionId, int.Parse(workspace), ipAddress, null, serAgent, requestId, HttpContext.RequestAborted);

                if (!response.Success)
                    return response.ErrorCode == "EXCEPTION_NOT_FOUND"
                        ? NotFound(response)
                        : BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Get all users that the caller can manage (admin scope overlap).
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("manageable-users")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetManageableUsersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetManageableUsersAsync()
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                var response = await userManagementService.GetManageableUsersAsync(
                     int.Parse(user), int.Parse(workspace), HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Check if the caller can manage a specific target user.
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("can-manage/{targetUserId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(CanManageUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CanManageUserAsync([FromRoute] int targetUserId)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                var response = await userManagementService.CanManageUserAsync(
                     int.Parse(user), targetUserId, int.Parse(workspace), HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Update Avatar
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Assignment not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("transfer-manager")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UpdateAvatarResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UpdateAvatarResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UpdateAvatarResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TransferUserManagerAsync([FromBody] TransferUserManagerRequest request)
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

                var response = await userManagementService.TransferUserManagerAsync(
                    int.Parse(workspace),
                    int.Parse(user),
                    request,
                    ipAddress,
                    null,
                    serAgent,
                    requestId,
                    HttpContext.RequestAborted);
                if (response == null || !response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

    }
}

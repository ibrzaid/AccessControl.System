using ACS.Helper.V1;
using Asp.Versioning;
using ACS.Models.Response;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ACS.Models.Response.V1.AuthenticationService.UserRole;
using ACS.Models.Request.V1.AuthenticationService.UserRole;
using ACS.Authentication.WebService.Services.V1.Interfaces;

namespace ACS.Authentication.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class RolesController(IUserRoleService service, FindClaimHelper findClaimHelper) : BaseController
    {
        /// <summary>
        /// List all active roles in the workspace.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetRolesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GetRolesResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetRolesAsync()
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                var response = await service.GetRolesAsync(int.Parse(workspace), 0, HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Get a single role by ID.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>        
        [HttpGet("{roleId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetRoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GetRoleResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetRolesResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRoleByIdAsync([FromRoute] int roleId)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                var response = await service.GetRoleByIdAsync(
                    roleId, int.Parse(workspace), HttpContext.RequestAborted);

                if (!response.Success || response.Data is null) return NotFound(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Create a new role. role_names must contain at least an "en"-US key.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="201">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="400">HTTP Bad Request</response>
        [HttpPost]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(RoleOperationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RoleOperationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRoleAsync(
            [FromBody] CreateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new RoleOperationResponse
                    {
                        Success   = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Error   = string.Join(", ", errors)
                    });
                }

                if (!request.RoleNames.TryGetValue("en-US", out var enName) || string.IsNullOrWhiteSpace(enName))
                {
                    return BadRequest(new RoleOperationResponse
                    {
                        Success   = false,
                        ErrorCode = "ROLE_NAME_REQUIRED",
                        Error   = "Role name (English) is required"
                    });
                }
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                var response = await service.CreateRoleAsync(
                    int.Parse(user), int.Parse(workspace), request,
                    ipAddress, serAgent, requestId,
                    HttpContext.RequestAborted);

                if (!response.Success) return BadRequest(response);
                return StatusCode(201, response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Update a role. Null fields are not changed.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="400">HTTP Bad Request</response>
        /// <response code="404">HTTP Not Found</response>
        [HttpPatch("{roleId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(RoleOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoleOperationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RoleOperationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRoleAsync(
            [FromRoute] int roleId,
            [FromBody] UpdateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new RoleOperationResponse
                    {
                        Success   = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Error   = string.Join(", ", errors)
                    });
                }
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                var response = await service.UpdateRoleAsync(
                     int.Parse(user), roleId, int.Parse(workspace), request,
                    ipAddress, serAgent, requestId,
                    HttpContext.RequestAborted);

                if (!response.Success)
                    return response.ErrorCode == "ROLE_NOT_FOUND"
                        ? NotFound(response) : BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Soft-delete a role and remove all user assignments.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="404">HTTP Not Found</response>
        [HttpDelete("{roleId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(RoleOperationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoleOperationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRoleAsync(
            [FromRoute] int roleId,
            [FromBody] DeleteRoleRequest? request = null)
        {
            try
            {
                string requestId = GetRequestId();
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string serAgent = Request.Headers.UserAgent.ToString();
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                var response = await service.DeleteRoleAsync(
                     int.Parse(user), roleId, int.Parse(workspace),
                    request ?? new DeleteRoleRequest(),
                    ipAddress, serAgent, requestId,
                    HttpContext.RequestAborted);

                if (!response.Success)
                    return response.ErrorCode == "ROLE_NOT_FOUND"
                        ? NotFound(response) : BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }
    }
}

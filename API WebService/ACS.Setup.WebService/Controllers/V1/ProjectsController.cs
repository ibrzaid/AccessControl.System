using ACS.Helper.V1;
using Asp.Versioning;
using ACS.BusinessEntities;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.Models.Response.V1.SetupService.Area;
using ACS.Models.Request.V1.SetupService.Project;
using ACS.Models.Response.V1.SetupService.Project;
using ACS.Setup.WebService.Services.V1.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ACS.Setup.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ProjectsController(IProjectService service, IAreaService areaService, FindClaimHelper findClaimHelper) : BaseController
    {
        /// <summary>
        /// Get list of projects in the workspace with pagination. Caller must have at least read access to the project to see it in the list.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetProjectsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetProjectsAsync(
            [FromQuery] int limit = 100,
            [FromQuery] int offset = 0)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                var response  = await service.GetProjectsAsync(workspace, caller, limit, offset, HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Get list of areas under the project. Caller must have at least read access to the project to see the areas in the list. This is to support the scenario where a project has too many areas and client want to load them in batch with pagination.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpGet("{projectId:int}/Areas")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetAreasResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAreasAsync(
            [FromRoute] int projectId,
            [FromQuery] int limit = 100,
            [FromQuery] int offset = 0)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                var response  = await areaService.GetAreasAsync(workspace, caller, projectId, limit, offset, HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        /// <summary>
        /// Create a new project. Caller must have admin access to the workspace to create a project. Project name must be unique under the workspace. Caller can specify the project name in multiple languages by passing in a dictionary with language code as key and localized name as value. If caller only pass in one name
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="201">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpPost]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationProjectResultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationProjectResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateProjectAsync([FromBody] CreateProjectRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationProjectResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace  = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller     = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip      = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua      = Request.Headers.UserAgent.ToString();
                string rid     = GetRequestId();

                var response = await service.CreateProjectAsync(workspace, caller, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return StatusCode(201, response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Update project details. Caller must have admin access to the project to update the project. Caller can update the project name, description, address, location, timezone, and whether the project is public or not. When updating the project name, caller can also specify the language code for the name to update the localized name for that language. If caller only pass in one name without language code, system will treat it as default name and update the default name of the project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpPatch("{projectId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationProjectResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationProjectResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateProjectAsync([FromRoute] int projectId, [FromBody] UpdateProjectRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new OperationProjectResultResponse { Success = false, ErrorCode = "VALIDATION_ERROR", Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });

                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.UpdateProjectAsync(workspace, caller, projectId, request, ip, ua, null, rid, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Delete a project. Caller must have admin access to the project to delete the project. When a project is deleted, all areas, zones, access points, and hardware under the project will also be deleted. Caller must provide the latitude and longitude of the project location when deleting the project for auditing purpose.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        /// <response code="200">HTTP OK, Return on Successful</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="500">HTTP Internal Server Error. Returned when the service faces internal errors.</response>
        /// <response code="400">HTTP Bad Request. Returned when the submitted request is invalid.</response>
        /// <response code="401">HTTP Unauthorized. Returned when the submitted request is invalid.</response>
        [HttpDelete("{projectId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(OperationProjectResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationProjectResultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteProjectAsync([FromRoute] int projectId,
            [FromQuery] string latitude, [FromQuery] string longitude)
        {
            try
            {
                int workspace = int.Parse(findClaimHelper.FindClaim(HttpContext, "wid"));
                int caller    = int.Parse(findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier));
                string ip     = HttpContext.Connection.RemoteIpAddress?.ToString()!;
                string ua     = Request.Headers.UserAgent.ToString();
                string rid    = GetRequestId();

                var response = await service.DeleteProjectAsync(workspace, caller, projectId, ip, ua, rid, latitude, longitude, HttpContext.RequestAborted);
                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }
    }
}

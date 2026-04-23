using ACS.Helper.V1;
using Asp.Versioning;
using ACS.BusinessEntities;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.Setup.WebService.Services.V1.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ACS.Models.Response.V1.SetupService.HierarchyAccess;

namespace ACS.Setup.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class HierarchyAccessController(IHierarchyAccessService service, FindClaimHelper findClaimHelper) : BaseController
    {
        /// <summary>
        /// Get the hierarchy tree for the calling user.
        /// The tree starts at the user's highest scope level and goes down
        /// to hardware. Use result_level to parse data[] correctly.
        /// </summary>
        /// <param name="projectId">Optional — narrow to one project.</param>
        /// <param name="projectAreaId">Optional — narrow to one project area.</param>
        /// <param name="zoneId">Optional — narrow to one zone.</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(HierarchyAccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetHierarchyTreeAsync(
            [FromQuery] int? projectId = null,
            [FromQuery] int? projectAreaId = null,
            [FromQuery] int? zoneId = null)
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);

                var response = await service.GetHierarchyAccessAsync(
                    int.Parse(user),
                    int.Parse(workspace),
                    projectId,
                    projectAreaId,
                    zoneId,
                    HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }
    }
}

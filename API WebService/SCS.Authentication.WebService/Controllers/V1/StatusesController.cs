using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.UserStatus;

namespace ACS.Authentication.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class StatusesController(IUserStatusService service) : BaseController
    {

        /// <summary>
        /// List all user status options (ACTIVE, INACTIVE, SUSPENDED, LOCKED …).
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GetStatusesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]       
        public async Task<IActionResult> GetUserStatusesAsync()
        {
            try
            {
                var response = await service.GetUserStatusesAsync(HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }
    }
}

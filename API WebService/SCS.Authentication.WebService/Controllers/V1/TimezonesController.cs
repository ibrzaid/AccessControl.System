using ACS.Helper.V1;
using Asp.Versioning;
using ACS.Models.Response;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.Timezone;

namespace ACS.Authentication.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class TimezonesController(ITimezoneService service, FindClaimHelper findClaimHelper) : BaseController
    {
        /// <summary>
        /// All PostgreSQL timezone names grouped by region.
        /// Pass search= to filter (e.g. "Riyadh", "Asia", "UTC+3").
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("timezones")]
        [AllowAnonymous]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetTimezonesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTimezonesAsync([FromQuery] string? search = null)
        {
            try
            {
                var response = await service.GetTimezonesAsync(search, HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Get the effective timezone, date format, and language for the caller.
        /// User setting → workspace fallback → UTC / YYYY-MM-DD / en-US.
        /// Call once on login and cache client-side.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("display-settings")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(DisplaySettingsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetDisplaySettingsAsync()
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                string user = findClaimHelper.FindClaim(HttpContext, ClaimTypes.NameIdentifier);
                var response = await service.GetDisplaySettingsAsync(
                    int.Parse(user), int.Parse(workspace), HttpContext.RequestAborted);

                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }
    }
}

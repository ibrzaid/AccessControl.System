using ACS.Helper.V1;
using Asp.Versioning;
using ACS.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.License;

namespace ACS.Authentication.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class LicenseController(ILicenseService service, FindClaimHelper findClaimHelper) : BaseController
    {

        /// <summary>
        /// Validate workspace license and available user seats.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("check")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(LicenseCheckResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CheckLicenseAsync()
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                var response = await service.CheckLicenseAsync(int.Parse(workspace), HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        /// <summary>
        /// Seat usage summary for the admin dashboard.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("seats")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(SeatSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSeatSummaryAsync()
        {
            try
            {
                string workspace = findClaimHelper.FindClaim(HttpContext, "wid");
                var response = await service.GetSeatSummaryAsync(int.Parse(workspace), HttpContext.RequestAborted);
                return Ok(response);
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

    }
}

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.Master.WebService.Services.V1.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ACS.Models.Response.V1.MasterService.SetupReference;

namespace ACS.Master.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class SetupReferenceController(ISetupReferenceService service) :  BaseController
    {
        [HttpGet("ProjectTypes")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetSetupReferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetProjectTypesAsync()
        {
            try { return Ok(await service.GetProjectTypesAsync(HttpContext.RequestAborted)); }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        [HttpGet("AreaTypes")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetSetupReferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAreaTypesAsync()
        {
            try { return Ok(await service.GetAreaTypesAsync(HttpContext.RequestAborted)); }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        [HttpGet("ZoneTypes")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetSetupReferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetZoneTypesAsync()
        {
            try { return Ok(await service.GetZoneTypesAsync(HttpContext.RequestAborted)); }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        [HttpGet("AccessPointTypes")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetSetupReferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAccessPointTypesAsync()
        {
            try { return Ok(await service.GetAccessPointTypesAsync(HttpContext.RequestAborted)); }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        [HttpGet("AccessLevels")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetSetupReferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAccessLevelsAsync()
        {
            try { return Ok(await service.GetAccessLevelsAsync(HttpContext.RequestAborted)); }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        [HttpGet("Statuses")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetSetupReferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetStatusesAsync()
        {
            try { return Ok(await service.GetStatusesAsync(HttpContext.RequestAborted)); }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        [HttpGet("HardwareTypes")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetSetupReferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetHardwareTypesAsync()
        {
            try { return Ok(await service.GetHardwareTypesAsync(HttpContext.RequestAborted)); }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        [HttpGet("HardwareStatuses")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetSetupReferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetHardwareStatusesAsync()
        {
            try { return Ok(await service.GetHardwareStatusesAsync(HttpContext.RequestAborted)); }
            catch (Exception ex) { return Problem(ex.Message); }
        }


        [HttpGet("Countries")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GetSetupReferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCountriesAsync()
        {
            try { return Ok(await service.GetCountriesAsync(HttpContext.RequestAborted)); }
            catch (Exception ex) { return Problem(ex.Message); }
        }
    }
}

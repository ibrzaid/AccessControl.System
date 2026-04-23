using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACS.ANPR.WebService.Services.V1.Interfaces;

namespace ACS.ANPR.WebService.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class HikVisionController(IHikVisionService anprProcessorService) : BaseController()
    {

        [HttpPost("{id}/anpr")]
        [AllowAnonymous]
        [Produces("application/xml")]
        [Consumes("application/xml", "multipart/form-data")]
        [Authorize(AuthenticationSchemes = "Basic")]
        public async Task<IActionResult> ReceiveANPREvent([FromRoute] string id)
        {
            try
            {
                Request.EnableBuffering();
                Request.Body.Position = 0;
                var form = await Request.ReadFormAsync();
                return await anprProcessorService.ProcessAsync(id, form, HttpContext.RequestAborted);
            }
            catch (Exception ex)
            {
                return anprProcessorService.GenerateErrorResponse($"Internal error: {ex.Message}");
            }
        }
    }

}

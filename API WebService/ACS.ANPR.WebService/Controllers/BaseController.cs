
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ACS.ANPR.WebService.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseController() { }

        /// <summary>
        /// Getting Claim
        /// </summary>
        /// <param name="claimName"></param>
        /// <returns></returns>
        protected string? FindClaim(string claimName)
        {
            if (HttpContext.User.Identity is not ClaimsIdentity claimsIdentity) return null;
            var claim = claimsIdentity.FindFirst(claimName);
            if (claim == null) return null;
            return claim.Value;
        }

        protected string GetRequestId()
        {
            if (HttpContext.Items.TryGetValue("RequestId", out var headerRequestId))
                return headerRequestId?.ToString()!;
            if (Request.Headers.TryGetValue("X-Request-ID", out var headerValues))
                return headerValues.FirstOrDefault()!;
            return Guid.NewGuid().ToString();
        }
    }
}

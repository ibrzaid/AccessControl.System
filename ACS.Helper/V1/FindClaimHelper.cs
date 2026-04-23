using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;


namespace ACS.Helper.V1
{
    public class FindClaimHelper
    {
        /// <summary>
        /// Find Claim
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="claimName"></param>
        /// <returns></returns>
        public string FindClaim(HttpContext httpContext, string claimName)
        {
            if (httpContext.User.Identity is not ClaimsIdentity claimsIdentity) return string.Empty;
            var claim = claimsIdentity.FindFirst(claimName);
            if (claim == null) return string.Empty;
            return claim.Value;
        }

        /// <summary>
        /// Find Claim
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="claimName"></param>
        /// <returns></returns>
        public string FindClaim(HubCallerContext httpContext, string claimName)
        {
            if (httpContext.User == null) return "";
            if (httpContext.User.Identity is not ClaimsIdentity claimsIdentity) return string.Empty;
            var claim = claimsIdentity.FindFirst(claimName);
            if (claim == null) return string.Empty;
            return claim.Value;
        }
    }
}

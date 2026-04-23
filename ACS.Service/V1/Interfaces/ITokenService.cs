

using ACS.Models.Response.V1.AuthenticationService.Account;
using Microsoft.AspNetCore.Http;

namespace ACS.Service.V1.Interfaces
{
    public interface ITokenService
    {


        string GenerateAccessToken(
            string userId,
            string sessionId,
            string clientId,
            string workspaceId,
            string userFullName,
            string userEmail,
            bool requiresMfa,
            string secretKey,
            string[] roles,
            List<string> permissions,
            DateTime tokenExpiresAt,
            string appVersion,
            int scopeTypeId,
            string accessLevel,
            bool isAdmin,
            List<int> projectIds);

        /// <summary>
        /// Checks if the current token is active based on account data and the HTTP context.
        /// </summary>
        /// <param name="httpContext">Contains information about the current HTTP request and response, aiding in the token validation process.</param>
        /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
        /// <returns>Returns true if the token is active; otherwise, it returns false.</returns>
        Task<SessionValidationResponse> IsCurrentActiveToken(HttpContext httpContext, CancellationToken cancellationToken);
    }
}

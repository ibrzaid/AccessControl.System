using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Helper;
using ACS.Helper.V1;
using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using Microsoft.AspNetCore.Http;
using ACS.Models.Response.V1.AuthenticationService.Account;

namespace ACS.Service.V1.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connections"></param>
    /// <param name="findClaimHelper"></param>
    public class TokenService(ILicenseManager license, FindClaimHelper findClaimHelper) : Service(license), ITokenService
    {
        public IAuthenticationDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.AuthenticationDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in AuthenticationService.")
                };
            }
        }

        public string GenerateAccessToken(string userId,
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
            List<int> projectIds) =>
            TokenHelper.GenerateAccessToken(userId, 
                sessionId,
                clientId,
                workspaceId,
                userFullName,
                userEmail,
                requiresMfa,
                secretKey,
                roles,
                permissions,
                tokenExpiresAt,
                appVersion,
                scopeTypeId,
                accessLevel,
                isAdmin,
                projectIds);



        public async  Task<SessionValidationResponse> IsCurrentActiveToken(HttpContext httpContext, CancellationToken cancellationToken)
        {
            var license = this.LicenseManager.GetLicense();
            string session = findClaimHelper.FindClaim(httpContext, "sid");
            string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString()!;
            string? serAgent = httpContext.Request.Headers["User-Agent"];
            string requestId = httpContext.Request.Headers["X-Request-ID"].FirstOrDefault()?? string.Empty;
            requestId = string.IsNullOrEmpty(requestId) ? httpContext.Request.Headers["RequestId"].FirstOrDefault()?? string.Empty : requestId;
            requestId = string.IsNullOrEmpty(requestId) ? Guid.NewGuid().ToString() : requestId;
            var response = await this[license?.DB!].ValidateSession(session, ipAddress, null, serAgent, requestId, 0 , 0, cancellationToken);
            return new()
            {
                ErrorCode = response.ErrorCode,
                Error=response.Message,
                ExpiresInSeconds = response.ExpiresInSeconds,
                IsValid = response.IsValid,
                Success = response.Success,
                UserData= response.UserData == null ? null : new SessionValidationUserProfileResponse
                {
                   ClientId= response.UserData.ClientId,
                   SessionId= response.UserData.SessionId,
                   Status= response.UserData.Status,
                   TokenExpiresAt = response.UserData.TokenExpiresAt,
                   Email= response.UserData.Email,
                   FullName= response.UserData.FullName,
                   UserId= response.UserData.UserId,
                   Username= response.UserData.Username,
                   WorkspaceId= response.UserData.WorkspaceId,
                   WorkspaceName = response.UserData.WorkspaceName,
                }

            };
        }
    }
}

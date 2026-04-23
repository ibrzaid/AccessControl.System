using ACS.License.V1;
using System.Globalization;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Models.Request.V1.AuthenticationService.Account;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.Account;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class AuthenticationService(ILicenseManager licenseManager, ITokenService token) : Service.Service(licenseManager), IAuthenticationService
    {
        

        /// <summary>
        /// Get DataAccess
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
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

        public async Task<LoginResponse> LoginAsync(LoginRequest request,string client,string? ipAddress,string? deviceInfo,string? agent,string? requestId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();

            if (!double.TryParse(request.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude  = 0;
            if (!double.TryParse(request.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;

            var response = await this[license?.DB!].LoginAsync(
                request.Username!, request.Password!, client,
                ipAddress, deviceInfo, agent, requestId,
                _latitude, _longitude, cancellationToken);

            string? accessToken = null;

            if (response.Success)
                accessToken = token.GenerateAccessToken(
                    response.User?.UserId.ToString()!,
                    response.SessionId!.Value.ToString(),
                    response.Client!.ClientId!,
                    response.User?.WorkspaceId.ToString()!,
                    response.User?.FullName!,
                    response.User?.Email!,
                    response.RequiresMfa,
                    response.Client.ClientSecret!,
                    [.. response.Roles.Select(r => r.RoleName!)],
                    response.Permissions,
                    response.TokenExpiresAt!.Value,
                    "1",
                    // ── New scope claims ──────────────────────────────────────────────
                    response.ScopeTypeId,
                    response.AccessLevel,
                    response.IsAdmin,
                    response.ProjectIds
                );

            return response.Success ? new()
            {
                Success            = response.Success,
                AccessToken        = accessToken,
                Client = new()
                {
                    AllowedScopes = response.Client?.AllowedScopes,
                    ClientId      = response.Client?.ClientId,
                    ClientName    = response.Client?.ClientName,
                    RedirectUris  = response.Client?.RedirectUris
                },
                ErrorCode          = response.ErrorCode,
                Error              = response.Message,
                ExpiresIn          = response.TokenExpiresAt.HasValue
                                     ? (long)(response.TokenExpiresAt.Value - DateTime.UtcNow).TotalSeconds
                                     : 0,
                FailedAttempts     = response.FailedAttempts,
                RemainingAttempts  = response.RemainingAttempts,
                Permissions        = response.Permissions,
                RefreshExpiresAt   = response.RefreshExpiresAt,
                RefreshToken       = response.RefreshToken,
                RequiresMfa        = response.RequiresMfa,
                Roles              = [.. (from r in response.Roles select new UserRoleResponse
        {
            RoleName    = r.RoleName,
            Permissions = r.Permissions,
            RoleId      = r.RoleId
        })],
                SessionId          = response.SessionId,
                TokenExpiresAt     = response.TokenExpiresAt,
                // ── Scope fields ──────────────────────────────────────────────────────
                ScopeTypeId        = response.ScopeTypeId,
                AccessLevel        = response.AccessLevel,
                IsAdmin            = response.IsAdmin,
                ProjectIds         = response.ProjectIds,
                User = new UserProfileResponse
                {
                    UserId      = response.User!.UserId,
                    Username    = response.User?.Username,
                    FullName    = response.User?.FullName,
                    Email       = response.User?.Email,
                    PhoneNumber = response.User?.PhoneNumber,
                    AvatarUrl   = response.User?.AvatarUrl,
                    JobTitle    = response.User?.JobTitle,
                    Department  = response.User?.Department,
                    Timezone    = response.User?.Timezone ?? "UTC",
                    Language    = response.User?.Language ?? "en-US",
                    MfaEnabled  = response.User?.MfaEnabled ?? false,
                    LastLogin   = response.User?.LastLogin,
                    WorkspaceId = response.User!.WorkspaceId,
                    WorkspaceName = response.User?.WorkspaceName
                }
            } : new()
            {
                Success   = response.Success,
                ErrorCode = response.ErrorCode,
                Error     = response.Message,
            };
        }


        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();

            if (!double.TryParse(request.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude  = 0;
            if (!double.TryParse(request.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;

            var response = await this[license?.DB!].RefreshTokenAsync(
                request.RefreshToken!, ipAddress, deviceInfo, agent, requestId,
                _latitude, _longitude, cancellationToken);

            string? accessToken = null;

            if (response.Success)
                accessToken = token.GenerateAccessToken(
                    response.User?.UserId.ToString()!,
                    response.SessionId!.Value.ToString(),
                    response.Client!.ClientId!,
                    response.User?.WorkspaceId.ToString()!,
                    response.User?.FullName!,
                    response.User?.Email!,
                    response.RequiresMfa,
                    response.Client.ClientSecret!,
                    [.. response.Roles.Select(r => r.RoleName!)],
                    response.Permissions,
                    response.TokenExpiresAt!.Value,
                    "1",
                    // ── New scope claims ──────────────────────────────────────────────
                    response.ScopeTypeId,
                    response.AccessLevel,
                    response.IsAdmin,
                    response.ProjectIds
                );

            return response.Success ? new()
            {
                Success            = response.Success,
                AccessToken        = accessToken,
                Client = new()
                {
                    AllowedScopes = response.Client?.AllowedScopes,
                    ClientId      = response.Client?.ClientId,
                    ClientName    = response.Client?.ClientName,
                    RedirectUris  = response.Client?.RedirectUris
                },
                ErrorCode          = response.ErrorCode,
                Error              = response.Message,
                ExpiresIn          = response.TokenExpiresAt.HasValue
                                     ? (long)(response.TokenExpiresAt.Value - DateTime.UtcNow).TotalSeconds
                                     : 0,
                FailedAttempts     = response.FailedAttempts,
                RemainingAttempts  = response.RemainingAttempts,
                Permissions        = response.Permissions,
                RefreshExpiresAt   = response.RefreshExpiresAt,
                RefreshToken       = response.RefreshToken,
                RequiresMfa        = response.RequiresMfa,
                Roles              = [.. (from r in response.Roles select new UserRoleResponse
        {
            RoleName    = r.RoleName,
            Permissions = r.Permissions,
            RoleId      = r.RoleId
        })],
                SessionId          = response.SessionId,
                TokenExpiresAt     = response.TokenExpiresAt,
                // ── Scope fields ──────────────────────────────────────────────────────
                ScopeTypeId        = response.ScopeTypeId,
                AccessLevel        = response.AccessLevel,
                IsAdmin            = response.IsAdmin,
                ProjectIds         = response.ProjectIds,
                User = new UserProfileResponse
                {
                    UserId        = response.User!.UserId,
                    Username      = response.User?.Username,
                    FullName      = response.User?.FullName,
                    Email         = response.User?.Email,
                    PhoneNumber   = response.User?.PhoneNumber,
                    AvatarUrl     = response.User?.AvatarUrl,
                    JobTitle      = response.User?.JobTitle,
                    Department    = response.User?.Department,
                    Timezone      = response.User?.Timezone ?? "UTC",
                    Language      = response.User?.Language ?? "en-US",
                    MfaEnabled    = response.User?.MfaEnabled ?? false,
                    LastLogin     = response.User?.LastLogin,
                    WorkspaceId   = response.User!.WorkspaceId,
                    WorkspaceName = response.User?.WorkspaceName
                }
            } : new()
            {
                Success   = response.Success,
                ErrorCode = response.ErrorCode,
                Error     = response.Message,
            };
        }

     

        public async Task<LogoutResponse> LogoutAsync( string session, string latitude, string longitude, string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            if (!double.TryParse(latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude = 0;
            if (!double.TryParse(longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;

            var response = await this[license?.DB!].LogoutAsync(session, ipAddress, deviceInfo, agent, requestId, _latitude, _longitude, cancellationToken);
            return new LogoutResponse
            {
                Success = response.Success,
                ErrorCode = response.ErrorCode,
                Error = response.Message
            };
        }
    }
}

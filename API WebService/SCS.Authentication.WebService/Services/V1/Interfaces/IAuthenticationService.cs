using ACS.Models.Request.V1.AuthenticationService.Account;
using ACS.Models.Response;
using ACS.Models.Response.V1.AuthenticationService.Account;

namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request, string client, string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default);

        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default);

        Task<LogoutResponse> LogoutAsync(string session, string latitude, string longitude, string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default);

        
    }
}

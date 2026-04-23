using ACS.Database.IDataAccess.SetupService.V1;
using ACS.Helper;
using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace ACS.ANPR.WebService.Handler.V1
{
    public class BasicAuthenticationHandler(ILicenseManager licenseManager, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        private const string SecretKey = "ACS001-@ANPR-Y2025-D0325";
        private readonly string _authAlgorithm = "MD5";

        private readonly Dictionary<string, string> _credentials
            = new(StringComparer.OrdinalIgnoreCase)
            {
                { "admin", "hik12345" },
            };


        private IHardwareDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.SetupService.V1.HardwareDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in BasicAuthenticationHandler.")
                };
            }
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var key = TokenHelper.GenerateSecretKey(SecretKey);
            try
            {
                if (!Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value))
                    return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));


                var validSchemes = new[] { "Basic ", "Digest " };
                var authorizationHeader = value.ToString();
                if (!validSchemes.Any(scheme => authorizationHeader.StartsWith(scheme, StringComparison.OrdinalIgnoreCase)))
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));

                var authData = authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase)
                    ? authorizationHeader["Basic ".Length..].Trim()
                    : authorizationHeader["Digest ".Length..].Trim();


                return authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase) ?
                     HandleBasicAuthentication(authData):
                     HandleDigestAuthentication(authData);
            }
            catch 
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));
            }
        }

        private async Task<AuthenticateResult> HandleDigestAuthentication(string authData)
        {
            var digest = ParseDigestHeader(authData);
            if (!digest.TryGetValue("username", out var hwSerial)  ||
                !digest.TryGetValue("realm", out var realm)     ||
                !digest.TryGetValue("nonce", out var nonce)     ||
                !digest.TryGetValue("uri", out var uri)       ||
                !digest.TryGetValue("response", out var clientHash)||
                !digest.TryGetValue("ap_prefix", out var apPrefix)  ||
                !digest.TryGetValue("ap_serial", out var apSerial)) return AuthenticateResult.Fail( "Digest must include ap_prefix and ap_serial fields");

            var license = licenseManager.GetLicense();
            var result = await this[license?.DB!].ResolveBySerialAsync(apPrefix, apSerial, hwSerial, Context.RequestAborted);
            if (result== null || result.Data== null || !result.Success) return AuthenticateResult.Fail("Invalid credentials");

            var ha1 = CalculateMD5Hash($"{hwSerial}:{realm}:{apPrefix}:{apSerial}");
            var ha2 = CalculateMD5Hash($"{Request.Method}:{uri}");
            var expected = CalculateMD5Hash($"{ha1}:{nonce}:{ha2}");

            if (!string.Equals(clientHash, expected, StringComparison.OrdinalIgnoreCase)) return AuthenticateResult.Fail("Invalid credentials");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name,       result.Data.HwSerialNumber  ?? ""),
                new("UserType",            "Camera"),
                new("AuthenticationType",  "Digest"),
                new("wid",                  result.Data.WorkspaceId.ToString()),
                new("workspace_code",       result.Data.WorkspaceCode ?? ""),
                new("hardware_id",         result.Data.HardwareId.ToString()),
                new("hardware_type",       result.Data.HardwareTypeCode ?? ""),
                new("hardware_status",     result.Data.HardwareStatusCode ?? ""),
                new("lane_number",         (result.Data.LaneNumber ?? 0).ToString()),
                new("access_point_id",     result.Data.AccessPointId.ToString()),
                new("access_point_prefix", result.Data.AccessPointPrefix ?? ""),
                new("access_point_serial", result.Data.AccessPointSerial ?? ""),
                new("access_point_type",   result.Data.AccessPointTypeCode ?? ""),
                new("zone_id",             result.Data.ZoneId.ToString()),
                new("project_area_id",     result.Data.ProjectAreaId.ToString()),
                new("project_id",          result.Data.ProjectId.ToString()),
                new("workspace_id",        result.Data.WorkspaceId.ToString()),
                new("confidence_threshold",
                    (result.Data.AnprConfidenceThreshold ?? 85).ToString()),
                new("timezone",            result.Data.Timezone ?? "UTC"),
            };
            var identity = new ClaimsIdentity(claims, "Digest");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Digest");
            return AuthenticateResult.Success(ticket);

        }

        private async Task<AuthenticateResult> HandleBasicAuthentication(string authData)
        {
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(authData));
            var credentials = decodedCredentials.Split(':');

           

            // username:password  — password itself contains ":" (prefix:serial)
            // Split on first ":" only → [hw_serial, ap_prefix:ap_serial]
            var colonIdx = decodedCredentials.IndexOf(':');
            if (colonIdx < 1) return AuthenticateResult.Fail("Invalid Basic credentials format");

            var hwSerial = decodedCredentials[..colonIdx].Trim();
            var password = decodedCredentials[(colonIdx + 1)..].Trim();
            
            var pwColonIdx = password.IndexOf(':');
            if (pwColonIdx < 1) return AuthenticateResult.Fail("Invalid password format. Expected: ap_prefix:ap_serial (e.g. CME875:HIK-GATE-001)");

            var apPrefix = password[..pwColonIdx].Trim();
            var apSerial = password[(pwColonIdx + 1)..].Trim();

            return await ResolveAndBuildTicketAsync(apPrefix, apSerial, hwSerial, "Basic");
        }


        private async Task<AuthenticateResult> ResolveAndBuildTicketAsync(string apPrefix, string apSerial, string hwSerial, string scheme)
        {
            var license = licenseManager.GetLicense();
            var result = await this[license?.DB!].ResolveBySerialAsync(apPrefix, apSerial, hwSerial, Context.RequestAborted);
            if (result== null || result.Data== null || !result.Success) return AuthenticateResult.Fail("Invalid credentials");
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name,       result.Data.HwSerialNumber  ?? ""),
                new("UserType",            "Camera"),
                new("AuthenticationType",  scheme),
                new("wid",                  result.Data.WorkspaceId.ToString()),
                new("workspace_code",       result.Data.WorkspaceCode ?? ""),
                new("hardware_id",         result.Data.HardwareId.ToString()),
                new("hardware_type",       result.Data.HardwareTypeCode ?? ""),
                new("hardware_status",     result.Data.HardwareStatusCode ?? ""),
                new("lane_number",         (result.Data.LaneNumber ?? 0).ToString()),
                new("access_point_id",     result.Data.AccessPointId.ToString()),
                new("access_point_prefix", result.Data.AccessPointPrefix ?? ""),
                new("access_point_serial", result.Data.AccessPointSerial ?? ""),
                new("access_point_type",   result.Data.AccessPointTypeCode ?? ""),
                new("zone_id",             result.Data.ZoneId.ToString()),
                new("project_area_id",     result.Data.ProjectAreaId.ToString()),
                new("project_id",          result.Data.ProjectId.ToString()),
                new("workspace_id",        result.Data.WorkspaceId.ToString()),
                new("confidence_threshold",
                    (result.Data.AnprConfidenceThreshold ?? 85).ToString()),
                new("timezone",            result.Data.Timezone ?? "UTC"),
            };
            var identity = new ClaimsIdentity(claims, scheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, scheme);
            return AuthenticateResult.Success(ticket);
        }


        private static string CalculateMD5Hash(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        private static Dictionary<string, string> ParseDigestHeader(string digestHeader)
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var parts = digestHeader.Split(',');
            foreach (var part in parts)
            {
                var keyValue = part.Trim().Split('=', 2);
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim().Trim('"');
                    parameters[key] = value;
                }
            }
            return parameters;
        }
    }
}

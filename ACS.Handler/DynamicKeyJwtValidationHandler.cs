using ACS.Helper;
using ACS.License.V1;
using System.Security.Claims;
using System.ComponentModel;
using ACS.Service.V1.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ACS.Database.IDataAccess.AuthenticationService.V1;


namespace ACS.Handler
{
    /// <summary>
    /// Validates a JWT token by dynamically generating a security key based on the 'client_id' claim. It configures
    /// validation parameters accordingly.
    /// </summary>
    public class DynamicKeyJwtValidationHandler(ILicenseManager licenseManager) : JwtSecurityTokenHandler, ISecurityTokenValidator
    {


        private IAuthenticationDataAccess this[Connection conn, string version]
        {
            get
            {
                return version switch
                {
                    "1" => conn.Type switch
                    {
                        Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.AuthenticationDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                        _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in AuthenticationService.")
                    },
                    _ => throw new NotSupportedException($"The version '{version}' is not supported in AuthenticationService.")
                };
            }
        }


        /// <summary>
        /// Validates a JWT token and configures validation parameters for security checks.
        /// </summary>
        /// <param name="token">The JWT token string that needs to be validated.</param>
        /// <param name="validationParameters">Contains settings that dictate how the token validation should be performed.</param>
        /// <param name="validatedToken">Outputs the security token that has been validated if the process is successful.</param>
        /// <returns>Returns a ClaimsPrincipal representing the user if validation is successful, otherwise null.</returns>
        public override ClaimsPrincipal? ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken? validatedToken)
        {
            try
            {
                var license = licenseManager.GetLicense();
                JwtSecurityToken incomingToken = ReadJwtToken(token);
                if (incomingToken != null)
                {
                    string client = incomingToken
                       .Claims
                       .First(claim => claim.Type == "cid")
                       .Value;

                    string version = incomingToken
                       .Claims
                       .FirstOrDefault(claim => claim.Type == "ver")?.Value ?? "1";


                    var secrect = this[license?.DB!, version].GetGetClientSecretByCLientIdAsync(client).GetAwaiter().GetResult();
                    var key = TokenHelper.GenerateSecretKey(secrect, 256);
                    var security = new SymmetricSecurityKey(key);
                    validationParameters.ValidateIssuerSigningKey = true;
                    validationParameters.IssuerSigningKey = security;
                    validationParameters.ValidateIssuer = false;
                    validationParameters.ValidateAudience = false;
                    validationParameters.ClockSkew = TimeSpan.Zero;
                }
            }
            catch { }
            return base.ValidateToken(token, validationParameters, out validatedToken);
        }
    }
}

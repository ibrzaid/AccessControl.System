using System.Text;
using System.Text.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ACS.Helper
{
    /// <summary>
    ///  Generate Access Token and Refresh Token
    /// </summary>
    public class TokenHelper
    {
        /// <summary>
        /// Security Token Issuer
        /// </summary>
        public const string Issuer = "http://acs.com";

        /// <summary>
        /// Security Token Audience
        /// </summary>
        public const string Audience = "http://acs.com";



        public static string GenerateAccessToken(
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
            List<int> projectIds)
        {

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
                throw new ArgumentException("Secret key must be at least 32 characters", nameof(secretKey));

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId),
                new("sid",                       sessionId),
                new("cid",                       clientId),
                new("wid",                       workspaceId),
                new("ver",                       appVersion),

                new(JwtRegisteredClaimNames.Name,  userFullName ?? string.Empty),
                new(JwtRegisteredClaimNames.Email, userEmail    ?? string.Empty),

                new("mfa", requiresMfa ? "true" : "false", ClaimValueTypes.Boolean),
 
                // ── Access scope claims ───────────────────────────────────────────────
                // scope_type_id: 4=workspace 3=project 2=area 1=zone 5=gate
                new("scope", scopeTypeId.ToString(), ClaimValueTypes.Integer32),
                // access_level: VIEW_ONLY | OPERATOR | SUPERVISOR | ADMIN
                new("lvl",   accessLevel ?? "VIEW_ONLY"),
                // is_admin shortcut — used by DashboardHub to join admins_{wid} group
                new("adm",   isAdmin ? "true" : "false", ClaimValueTypes.Boolean),

                new(JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString("N").ToUpperInvariant()),
                new(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            // Roles → ClaimTypes.Role (one claim per role)
            foreach (var role in roles)
                if (!string.IsNullOrWhiteSpace(role))
                    claims.Add(new Claim(ClaimTypes.Role, role));

            // Flat permissions array → single JSON claim
            if (permissions is { Count: > 0 })
                claims.Add(new Claim("perms",
                    JsonSerializer.Serialize(permissions),
                    JsonClaimValueTypes.JsonArray));

            // Project IDs → single JSON claim
            // e.g. "pids": [1, 3, 7]
            // Empty array for workspace admins means "all projects" (use isAdmin check instead)
            claims.Add(new Claim("pids",
                JsonSerializer.Serialize(projectIds ?? []),
                JsonClaimValueTypes.JsonArray));

            var key = GenerateSecretKey(secretKey, 256);
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject            = new ClaimsIdentity(claims),
                Expires            = tokenExpiresAt,
                NotBefore          = DateTime.UtcNow,
                Issuer             = Issuer,
                Audience           = Audience,
                SigningCredentials  = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }


        
        /// <summary>
        /// Generate Refresh Token
        /// </summary>
        /// <returns>Refresh Token</returns>
        public static async Task<string> GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[32];

            using var randomNumberGenerator = RandomNumberGenerator.Create();
            await Task.Run(() => randomNumberGenerator.GetBytes(secureRandomBytes));

            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            return refreshToken;
        }


        /// <summary>
        /// Generate Secert Key
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static byte[] GenerateSecretKey(string input, int keySize = 256)
        {
            // Ensure the key size is valid (256, 384, or 512 bits)
            if (keySize != 256 && keySize != 384 && keySize != 512)
            {
                throw new ArgumentException("Key size must be 256, 384, or 512 bits.");
            }

            // Create a hash of the input string
            // Compute the hash
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));

            // If the hash is longer than the desired key size, truncate it
            if (hash.Length * 8 > keySize)
            {
                Array.Resize(ref hash, keySize / 8);
            }

            return hash;

        }
    }
}

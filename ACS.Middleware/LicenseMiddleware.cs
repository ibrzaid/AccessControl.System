using System.Text.Json;
using ACS.Service.V1.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ACS.Middleware
{
    public class LicenseMiddleware(RequestDelegate next, ILicenseManager licenseManager)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task Invoke(HttpContext context)
        {
            if (ShouldSkipLicenseCheck(context))
            {
                await next(context);
                return;
            }

            if (!licenseManager.IsValid())
            {
                await HandleInvalidLicenseAsync(context);
                return;
            }

            AddLicenseHeaders(context);

            await next(context);
        }



        private static bool ShouldSkipLicenseCheck(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Skip for public endpoints
            return path != null && (
                path.StartsWith("/health") ||
                path.StartsWith("/status") ||
                path.StartsWith("/public/") ||
                path.StartsWith("/swagger") ||
                path.StartsWith("/favicon.ico")
            );
        }


        private async Task HandleInvalidLicenseAsync(HttpContext context)
        {
            var status = licenseManager.GetStatus();
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "License validation failed",
                message = status.Message,
                license = new
                {
                    valid = status.IsValid,
                    expired = status.IsExpired,
                    expiryDate = status.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ssZ")
                },
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                path = context.Request.Path.Value
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
        }


        private void AddLicenseHeaders(HttpContext context)
        {
            if (context.Request.Path.Value?.Contains("/api/", StringComparison.OrdinalIgnoreCase) != true)
                return;

            var status = licenseManager.GetStatus();

            context.Response.Headers["X-License-Valid"] = status.IsValid.ToString().ToLowerInvariant();
            context.Response.Headers["X-License-Expired"] = status.IsExpired.ToString().ToLowerInvariant();
            context.Response.Headers["X-License-Expiry"] = status.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
            context.Response.Headers["X-License-Days-Remaining"] =  status.DaysRemaining.ToString();

        }

    }
}

using StackExchange.Redis;
using ACS.Service.V1.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ACS.HealthChecks
{
    public class RedisHealthCheck(ILicenseManager licenseManager) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var license = licenseManager.GetLicense();

                // Check if Redis is configured in license
                if (license?.Cache == null || string.IsNullOrWhiteSpace(license.Cache.Connection))
                {
                    return HealthCheckResult.Healthy("Redis cache is not configured (optional component).");
                }

                // Create Redis connection
                using var redis = await ConnectionMultiplexer.ConnectAsync(
                    license.Cache.Connection);

                var db = redis.GetDatabase();

                // Test connection with a simple ping
                var pingResult = await db.PingAsync();

                return HealthCheckResult.Healthy(
                    $"Redis cache is healthy. Ping: {pingResult.TotalMilliseconds:F2}ms");
            }
            catch (RedisConnectionException rcex)
            {
                return HealthCheckResult.Unhealthy(
                    $"Redis connection failed: {rcex.Message}", rcex);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "An error occurred while checking Redis cache health.", ex);
            }
        }
    }
}
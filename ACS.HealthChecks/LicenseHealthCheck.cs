using ACS.Service.V1.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ACS.HealthChecks
{
#pragma warning disable CS8601 // Possible null reference assignment.
    public class LicenseHealthCheck(ILicenseManager licenseManager, ILogger<LicenseHealthCheck> logger) : IHealthCheck
    {
        private HealthCheckResult? _cachedResult;
        private DateTime _lastCacheTime = DateTime.MinValue;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(30);

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var cached = _cachedResult;
                if (_cachedResult != null && DateTime.UtcNow - _lastCacheTime < _cacheDuration)  return Task.FromResult((HealthCheckResult)_cachedResult);
                

                var status = licenseManager.GetStatus();
                var license = licenseManager.GetLicense();
                var licensePath = licenseManager.GetLicensePath();

                FileInfo? fileInfo = null;
                if (File.Exists(licensePath)) fileInfo = new FileInfo(licensePath);


                var data = new Dictionary<string, object>
                {
                    ["license"] = new
                    {
                        valid = status.IsValid,
                        expired = status.IsExpired,
                        aboutToExpire = status.IsAboutToExpire,
                        daysRemaining = status.DaysRemaining,
                        hoursRemaining = status.HoursRemaining,
                        message = status.Message,
                        description = license?.Description
                    },
                    ["file"] = new
                    {
                        path = licensePath,
                        exists = File.Exists(licensePath),
                        lastModified = fileInfo?.LastWriteTimeUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        sizeBytes = fileInfo?.Length,
                        sizeKB = fileInfo != null ? Math.Round(fileInfo.Length / 1024.0, 2) : 0
                    },
                    ["dates"] = new
                    {
                        issueDate = status.IssueDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        expiryDate = status.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        currentTime = status.CurrentTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        timeToExpiry = (status.ExpiryDate - status.CurrentTime).ToString(@"dd\.hh\:mm\:ss")
                    },
                    ["system"] = license?.System != null ? new
                    {
                        issuedDate = license.System.IssuedDate,
                        issuedTime = license.System.IssuedTime,
                        expiryDate = license.System.ExpiryDate,
                        expiryTime = license.System.ExpiryTime
                    } : null,
                    ["validation"] = new
                    {
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        cacheAge = (DateTime.UtcNow - _lastCacheTime).TotalSeconds
                    }
                };


                HealthCheckResult result;
                if (!status.IsValid)
                {
                    result = HealthCheckResult.Unhealthy(description: $"License invalid: {status.Message}",data: data);
                    logger.LogError("License health check failed: {Message}", status.Message);
                }
                else if (status.IsExpired)
                {
                    result = HealthCheckResult.Unhealthy(description: $"License expired on {status.ExpiryDate:yyyy-MM-dd HH:mm:ss} UTC", data: data);
                    logger.LogError("License expired: Expired on {ExpiryDate}", status.ExpiryDate);
                }
                else if (status.IsAboutToExpire)
                {
                    result = HealthCheckResult.Degraded(description: $"License expires in {status.DaysRemaining} day(s) ({status.ExpiryDate:yyyy-MM-dd HH:mm:ss} UTC)",data: data);
                    if (status.DaysRemaining <= 1)logger.LogCritical("License critical: Expires in {HoursRemaining} hours", status.HoursRemaining);                    
                    else if (status.DaysRemaining <= 3)logger.LogWarning("License warning: Expires in {DaysRemaining} days", status.DaysRemaining);                    
                    else logger.LogInformation("License expiring soon: {DaysRemaining} days remaining", status.DaysRemaining);                    
                }
                else
                {
                    result = HealthCheckResult.Healthy(description: $"License valid until {status.ExpiryDate:yyyy-MM-dd HH:mm:ss} UTC ({status.DaysRemaining} days remaining)", data: data);
                    if (status.DaysRemaining <= 30)  logger.LogInformation("License: {DaysRemaining} days remaining", status.DaysRemaining);                    
                }

                // Cache the result
                _cachedResult = result;
                _lastCacheTime = DateTime.UtcNow;

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "License health check encountered an error");

                var errorData = new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["errorType"] = ex.GetType().Name,
                    ["stackTrace"] = ex.StackTrace ?? string.Empty,
                    ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Task.FromResult(HealthCheckResult.Unhealthy(
                    description: "License health check error",
                    exception: ex,
                    data: errorData));
            }
        }
    }
#pragma warning restore CS8601 // Possible null reference assignment.
}

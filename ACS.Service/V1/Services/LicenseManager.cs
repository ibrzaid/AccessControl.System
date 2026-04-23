using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using Microsoft.Extensions.Logging;



namespace ACS.Service.V1.Services
{
    public class LicenseManager : ILicenseManager
    {
        private License.V1.License? _license;
        private readonly object _lock = new();

        private readonly ILogger<LicenseManager> _logger;
        private readonly string _licensePath;
        private LicenseStatus? _cachedStatus;
        private DateTime _lastStatusCheck = DateTime.MinValue;
        private readonly TimeSpan _statusCacheDuration = TimeSpan.FromSeconds(30);

        public LicenseManager(ILogger<LicenseManager> logger)
        {
            _logger = logger;
            _licensePath = Environment.GetEnvironmentVariable("LICENSE_PATH") ?? "/app/license/license.lic";
            var directory = Path.GetDirectoryName(_licensePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation("Created license directory: {directory}", directory);
            }
            InitializeLicense();
        }

        private void InitializeLicense()
        {
            lock (_lock)
            {
                if (_license != null) return;

                try
                {
                    _logger.LogInformation("Loading license from: {LicensePath}", _licensePath);
                    _license = ACS.License.V1.License.LoadAndValidate(_licensePath);
                    if (_license.IsValid) _logger.LogInformation("✓ License loaded. Valid until: {Expiry} UTC", _license.ExpiryDate.ToUniversalTime());
                    else _logger.LogCritical("✗ License validation failed: {Reason}", _license.Description);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Failed to load license");
                    _license = new ACS.License.V1.License {  IsValid = false,  Description = "License load failed" };
                }
            }
        }

        // Fast check – called per request
        public bool IsValid()
        {
            var license = _license;
            if (license == null) return false;
            return license.IsValid && !license.IsExpired();
        }

        // Detailed status (cached for 30 seconds)
        public LicenseStatus GetStatus()
        {
            if (DateTime.UtcNow - _lastStatusCheck < _statusCacheDuration && _cachedStatus != null) return _cachedStatus;            

            if (_license == null)  _cachedStatus = new LicenseStatus { IsValid = false, IsExpired = true, Message = "License not loaded" }; 
            else _cachedStatus = _license.GetStatus(); 
            _lastStatusCheck = DateTime.UtcNow;
            return _cachedStatus;
        }

        public License.V1.License? GetLicense() =>  _license;

        public string GetLicensePath() => _licensePath;
    }
}
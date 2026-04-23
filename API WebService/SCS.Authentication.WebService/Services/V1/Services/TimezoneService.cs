using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.Timezone;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class TimezoneService(ILicenseManager licenseManager) : Service.Service(licenseManager), ITimezoneService
    {
        public ITimezoneDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.TimezoneDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in TimezoneService.")
                };
            }
        }


        public async Task<GetTimezonesResponse> GetTimezonesAsync(string? search = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetTimezonesAsync(search, cancellationToken);
            return new GetTimezonesResponse
            {
                Success = true,
                Data    = [.. db.Select(t => new TimezoneResponse
                {
                    Name           = t.Name,
                    Abbrev         = t.Abbrev,
                    UtcOffset      = t.UtcOffset,
                    UtcOffsetLabel = t.UtcOffsetLabel,
                    IsDst          = t.IsDst,
                    Region         = t.Region
                })]
            };
        }

        public async Task<DisplaySettingsResponse> GetDisplaySettingsAsync(int userId, int workspaceId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetDisplaySettingsAsync(userId, workspaceId, cancellationToken);
            return new DisplaySettingsResponse
            {
                Success             = db.Success,
                UserTimezone        = db.UserTimezone,
                WorkspaceTimezone   = db.WorkspaceTimezone,
                EffectiveTimezone   = db.EffectiveTimezone,
                WorkspaceDateFormat = db.WorkspaceDateFormat,
                WorkspaceLanguage   = db.WorkspaceLanguage,
                UserLanguage        = db.UserLanguage,
                EffectiveLanguage   = db.EffectiveLanguage,
                CurrentTimeUtc      = db.CurrentTimeUtc,
                CurrentTimeLocal    = db.CurrentTimeLocal
            };
        }

        
    }
}

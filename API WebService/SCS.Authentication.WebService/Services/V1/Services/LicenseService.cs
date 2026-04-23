using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.License;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class LicenseService(ILicenseManager licenseManager) : Service.Service(licenseManager), ILicenseService
    {
        public ILicenseDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.LicenseDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in LicenseService.")
                };
            }
        }

        public async Task<LicenseCheckResponse> CheckLicenseAsync(int workspaceId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].CheckUserLicenseAsync(workspaceId, cancellationToken);
            return new LicenseCheckResponse
            {
                Success             = true,
                IsValid             = db.IsValid,
                Errors              = db.Errors,
                Warnings            = db.Warnings,
                CurrentUsers        = db.CurrentUsers,
                MaxUsers            = db.MaxUsers,
                RemainingSlots      = db.RemainingSlots,
                WorkspaceTimezone   = db.WorkspaceTimezone,
                WorkspaceDateFormat = db.WorkspaceDateFormat
            };
        }

        public async Task<SeatSummaryResponse> GetSeatSummaryAsync(int workspaceId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetUserSeatSummaryAsync(workspaceId, cancellationToken);
            return new SeatSummaryResponse
            {
                Success        = db.Success,
                MaxUsers       = db.MaxUsers,
                CurrentUsers   = db.CurrentUsers,
                RemainingSlots = db.RemainingSlots,
                UsagePct       = db.UsagePct,
                LicenseStatus  = db.LicenseStatus,
                ContractEnd    = db.ContractEnd,
                DaysToExpiry   = db.DaysToExpiry,
                ByStatus       = db.ByStatus
            };
        }
    }
}

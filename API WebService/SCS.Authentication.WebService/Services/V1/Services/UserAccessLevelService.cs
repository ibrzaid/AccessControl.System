using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.AccessLevel;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class UserAccessLevelService(ILicenseManager licenseManager) : Service.Service(licenseManager), IUserAccessLevelService
    {
        public IUserAccessLevelDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.UserAccessLevelDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserAccessLevelService.")
                };
            }
        }

        public async Task<GetAccessLevelsResponse> GetAccessLevelsAsync(CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetAccessLevelsAsync(cancellationToken);
            return new GetAccessLevelsResponse
            {
                Success = true,
                Data    = [.. db.Select(l => new AccessLevelResponse
                {
                    AccessLevelId    = l.AccessLevelId,
                    AccessLevelCode  = l.AccessLevelCode,
                    AccessLevelNames = l.AccessLevelNames,
                    Priority         = l.Priority,
                    Description      = l.Description
                })]
            };
        }
    }
}

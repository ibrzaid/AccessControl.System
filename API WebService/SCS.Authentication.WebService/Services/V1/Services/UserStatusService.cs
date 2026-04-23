using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.UserStatus;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class UserStatusService(ILicenseManager licenseManager) : Service.Service(licenseManager), IUserStatusService
    {

        public IUserStatusDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.UserStatusDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserStatusService.")
                };
            }
        }

        public async Task<GetStatusesResponse> GetUserStatusesAsync(CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetUserStatusesAsync(cancellationToken);
            return new GetStatusesResponse
            {
                Success = true,
                Data    = [.. db.Select(s => new UserStatusResponse
                {
                    UserStatusId    = s.UserStatusId,
                    UserStatusCode  = s.UserStatusCode,
                    UserStatusNames = s.UserStatusNames
                })]
            };
        }
    }
}

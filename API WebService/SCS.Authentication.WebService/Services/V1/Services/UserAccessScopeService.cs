using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.AccessScope;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class UserAccessScopeService(ILicenseManager licenseManager) : Service.Service(licenseManager), IUserAccessScopeService
    {
        public IUserAccessScopeDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.UserAccessScopeDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserAccessScopeService.")
                };
            }
        }

        public async Task<GetAccessScopesResponse> GetAccessScopesAsync(CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetAccessScopesAsync(cancellationToken);
            return new GetAccessScopesResponse
            {
                Success = true,
                Data    = [.. db.Select(s => new AccessScopeResponse
                {
                    ScopeTypeId   = s.ScopeTypeId,
                    ScopeTypeCode = s.ScopeTypeCode,
                    ScopeName     = s.ScopeName,
                    ScopeLevel    = s.ScopeLevel,
                    Description   = s.Description
                })]
            };
        }
    }
}

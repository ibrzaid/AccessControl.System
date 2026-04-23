using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.BusinessEntities.AuthenticationService.V1.UseRole;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.License.V1;
using ACS.Models.Request.V1.AuthenticationService.UserRole;
using ACS.Models.Response.V1.AuthenticationService.UserRole;
using ACS.Service.V1.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class UserRoleService(ILicenseManager licenseManager) : Service.Service(licenseManager), IUserRoleService
    {
        public IUserRoleDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.UserRoleDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserRoleService.")
                };
            }
        }


        public async Task<GetRolesResponse> GetRolesAsync(
            int workspaceId, int roleId = 0,
            CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetRolesAsync(workspaceId, roleId, cancellationToken);
            return new GetRolesResponse
            {
                Success   = db.Success,
                Error   = db.Message,
                ErrorCode = db.ErrorCode,
                Total     = db.Total,
                Data      = [.. db.Data.Select(MapRole)]
            };
        }


        public async Task<GetRoleResponse> GetRoleByIdAsync(
            int roleId, int workspaceId,
            CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetRolesAsync(workspaceId, roleId, cancellationToken);
            return new GetRoleResponse
            {
                Success   = db.Success,
                Error   = db.Message,
                ErrorCode = db.Total == 0 ? "ROLE_NOT_FOUND" : db.ErrorCode,
                Data      = db.Data.Count > 0 ? MapRole(db.Data[0]) : null
            };
        }

        public async Task<RoleOperationResponse> CreateRoleAsync(
            int callerUserId, int workspaceId,
            CreateRoleRequest request,
            string? ipAddress, string? userAgent, string? requestId,
            CancellationToken cancellationToken = default)
        {
            if (!decimal.TryParse(request.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _latitude)) _latitude  = 0;
            if (!decimal.TryParse(request.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _longitude)) _longitude = 0;

            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].CreateRoleAsync(
                callerUserId, workspaceId,
                JsonSerializer.Serialize(request.RoleNames),
                request.RoleDescription,
                JsonSerializer.Serialize(request.RolePermissions),
                ipAddress, userAgent, requestId, _latitude, _longitude,
                cancellationToken);

            return MapOperation(db);
        }


        public async Task<RoleOperationResponse> UpdateRoleAsync(
            int callerUserId, int roleId, int workspaceId,
            UpdateRoleRequest request,
            string? ipAddress, string? userAgent, string? requestId,
            CancellationToken cancellationToken = default)
        {
            if (!decimal.TryParse(request.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _latitude)) _latitude  = 0;
            if (!decimal.TryParse(request.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _longitude)) _longitude = 0;

            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].UpdateRoleAsync(
                callerUserId, roleId, workspaceId,
                request.RoleNames is not null ? JsonSerializer.Serialize(request.RoleNames) : null,
                request.RoleDescription,
                request.RolePermissions is not null ? JsonSerializer.Serialize(request.RolePermissions) : null,
                ipAddress, userAgent, requestId, _latitude, _longitude,
                cancellationToken);

            return MapOperation(db);
        }


        public async Task<RoleOperationResponse> DeleteRoleAsync(
            int callerUserId, int roleId, int workspaceId,
            DeleteRoleRequest request,
            string? ipAddress, string? userAgent, string? requestId,
            CancellationToken cancellationToken = default)
        {
            if (!decimal.TryParse(request.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _latitude)) _latitude  = 0;
            if (!decimal.TryParse(request.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _longitude)) _longitude = 0;

            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].DeleteRoleAsync(
                callerUserId, roleId, workspaceId, request.Reason,
                ipAddress, userAgent, requestId, _latitude, _longitude,
                cancellationToken);

            return MapOperation(db);
        }


        private static RoleResponse MapRole(UserRoleEntity e) => new()
        {
            UserRoleId      = e.UserRoleId,
            WorkspaceId     = e.WorkspaceId,
            RoleNames       = e.RoleNames,
            RoleDescription = e.RoleDescription,
            RolePermissions = e.RolePermissions,
            IsActive        = e.IsActive,
            CreatedDate     = e.CreatedDate,
            UpdatedDate     = e.UpdatedDate,
            UserCount       = e.UserCount
        };

        private static RoleOperationResponse MapOperation(UserRoleOperationResultEntity db) => new()
        {
            Success   = db.Success,
            Error   = db.Message,
            ErrorCode = db.ErrorCode,
            Data      = db.Data
        };
    }
}


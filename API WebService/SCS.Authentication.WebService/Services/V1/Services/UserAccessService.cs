using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.BusinessEntities.AuthenticationService.V1.UserAccess;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.License.V1;
using ACS.Models.Request.V1.AuthenticationService.AccessUser;
using ACS.Models.Response.V1.AuthenticationService.UserAccess;
using ACS.Models.Response.V1.AuthenticationService.UserManagement;
using ACS.Service.V1.Interfaces;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class UserAccessService(ILicenseManager licenseManager) : Service.Service(licenseManager), IUserAccessService
    {
        public IUserAccessDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.UserAccessDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserAccessService.")
                };
            }
        }

        public  async Task<GetAccessHistoryResponse> GetAccessHistoryAsync(int workspaceId, int userId = 0, int callerUserId = 0, int limit = 100, int offset = 0, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetAccessHistoryAsync(
                workspaceId, userId, callerUserId, limit, offset, cancellationToken);

            return new GetAccessHistoryResponse
            {
                Success = true,
                Data = [.. db.Select(a => new ActivityLogResponse
                {
                   LogId           = a.LogId ?? 0,
                   UserId          = a.UserId,
                   Action          = a.Action,
                   EntityType      = a.EntityType,
                   ResourceName    = a.ResourceName,
                   Description     = a.Description,
                   OldValues       = a.OldValues,
                   NewValues       = a.NewValues,
                   PerformedBy     = a.PerformedBy,
                   PerformedByName = a.PerformedByName,
                   CreatedAt       = a.CreatedAt,
                   ProjectId       = a.ProjectId,
                   ProjectAreaId   = a.ProjectAreaId,
                   ZoneId          = a.ZoneId,
                   AccessPointId   = a.AccessPointId,
                })]
            };
        }

        public async Task<GrantAccessResponse> GrantAccessAsync(int callerUserId, int workspaceId, GrantAccessRequest request, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GrantAccessAsync(
                grantedBy: callerUserId,
                userId: request.UserId,
                workspaceId: workspaceId,
                scopeTypeId: request.ScopeTypeId,
                accessLevelId: request.AccessLevelId,
                projectId: request.ProjectId,
                projectAreaId: request.ProjectAreaId,
                zoneId: request.ZoneId,
                accessPointId: request.AccessPointId,
                isAdminScope: request.IsAdminScope,
                isInherited: request.IsInherited,
                expiresAt: request.ExpiresAt,
                notes: request.Notes,
                cancellationToken: cancellationToken);

            return new GrantAccessResponse
            {
                Success      = true,
                UserAccessId = db.UserAccessId,
                Action       = db.Action,
                Error      = db.Action switch
                {
                    "inserted" => "Access granted successfully",
                    "upgraded" => "Access level upgraded successfully",
                    "reactivated" => "Access reactivated successfully",
                    "unchanged" => "No change — user already has equal or higher access at this scope",
                    _ => "Access processed"
                }
            };
        }

        public async Task<UserOperationResponse> RevokeAccessAsync(int callerUserId, long userAccessId, int workspaceId, RevokeAccessRequest request, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].RevokeAccessAsync(
                userAccessId, callerUserId, workspaceId, request.Reason, cancellationToken);

            return MapOperation(db);
        }


        private static UserOperationResponse MapOperation(AccessOperationResultEntity db) => new()
        {
            Success   = db.Success,
            Error   = db.Message,
            ErrorCode = db.ErrorCode,
            Data      = db.Data
        };
    }
}

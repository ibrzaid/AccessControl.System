using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.AccessRule;
using ACS.BusinessEntities.AuthenticationService.V1.AccessRule;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class UserAccessRuleService(ILicenseManager licenseManager) : Service.Service(licenseManager), IUserAccessRuleService
    {
        public IUserAccessRuleDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.UserAccessRuleDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserAccessRuleService.")
                };
            }
        }

        /// <summary>
        /// Get all active access rules. userId=0 → all users in workspace.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="callerUserId"></param>
        /// <param name="userId"></param>
        /// <param name="includeBreadcrumbs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<GetAccessRulesResponse> GetAccessRulesAsync(
            int workspaceId,
            int callerUserId,
            int userId = 0, 
            bool includeBreadcrumbs = false, 
            CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetAccessRulesAsync(
                workspaceId, 
                callerUserId, 
                userId, 
                includeBreadcrumbs, 
                cancellationToken);

            return new GetAccessRulesResponse
            {
                Success = true,
                Data    = [.. db.Select(MapAccessRule)]
            };
        }


        private static AccessRuleResponse MapAccessRule(UserAccessRuleEntity r) => new()
        {
            UserAccessId    = r.UserAccessId,
            UserId          = r.UserId,
            ScopeCode       = r.ScopeCode,
            ScopeLevel      = r.ScopeLevel,
            AccessLevelCode = r.AccessLevelCode,
            IsAdminScope    = r.IsAdminScope,
            VisibilityOnly  = r.VisibilityOnly,
            IsInherited     = r.IsInherited,
            IsExclusive     = r.IsExclusive,
            InheritDownTo   = r.InheritDownTo,
            ProjectId       = r.ProjectId,
            ProjectAreaId   = r.ProjectAreaId,
            ZoneId          = r.ZoneId,
            AccessPointId   = r.AccessPointId,
            ResourceName    = r.ResourceName,
            ExpiresAt       = r.ExpiresAt,
            GrantedAt       = r.GrantedAt,
            GrantedBy       = r.GrantedBy,
            Notes           = r.Notes,
            User            = r.User == null ? null : new AccessUserResponse
            {
               AvatarUrl = r.User.AvatarUrl,
               Email = r.User.Email,
               FullName = r.User.FullName,
               UserId = r.User.UserId,
               Username = r.User.Username
            }

        };
    }
}

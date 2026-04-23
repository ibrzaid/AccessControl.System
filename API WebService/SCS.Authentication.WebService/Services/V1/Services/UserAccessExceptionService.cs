using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.BusinessEntities.AuthenticationService.V1.UserAccess;
using ACS.Models.Request.V1.AuthenticationService.AccessException;
using ACS.Models.Response.V1.AuthenticationService.UserManagement;
using ACS.Models.Response.V1.AuthenticationService.UserAccessException;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class UserAccessExceptionService(ILicenseManager licenseManager) : Service.Service(licenseManager), IUserAccessExceptionService
    {
        public IUserAccessExceptionDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.UserAccessExceptionDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserAccessExceptionService.")
                };
            }
        }

        public async Task<GetAccessExceptionsResponse> GetAccessExceptionsAsync(int workspaceId, int userId = 0,   CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetAccessExceptionsAsync(workspaceId, userId, cancellationToken);
            return new GetAccessExceptionsResponse
            {
                Success = true,
                Data    = [.. db.Select(e => new AccessExceptionResponse
                {
                    AccessExceptionId    = e.AccessExceptionId,
                    UserAccessId         = e.UserAccessId,
                    UserId               = e.UserId,
                    ExceptionScopeTypeId = e.ExceptionScopeTypeId,
                    ExceptionResourceId  = e.ExceptionResourceId,
                    IsAllowed            = e.IsAllowed,
                    ExceptionReason      = e.ExceptionReason,
                    CreatedBy            = e.CreatedBy,
                    CreatedAt            = e.CreatedAt
                })]
            };
        }

        public async Task<UserOperationResponse> AddAccessExceptionAsync(int callerUserId, int workspaceId, AddExceptionRequest request, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var exceptionId = await this[license?.DB!].AddAccessExceptionAsync(
                createdBy: callerUserId,
                userAccessId: request.UserAccessId,
                userId: request.UserId,
                exceptionScopeTypeId: request.ExceptionScopeTypeId,
                exceptionResourceId: request.ExceptionResourceId,
                isAllowed: request.IsAllowed,
                reason: request.Reason,
                cancellationToken: cancellationToken);

            return new UserOperationResponse
            {
                Success = true,
                Error = "Exception created successfully",
                Data    = new { access_exception_id = exceptionId }
            };
        }

        public async Task<UserOperationResponse> DeleteAccessExceptionAsync(int callerUserId, long exceptionId, int workspaceId, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].DeleteAccessExceptionAsync(
                exceptionId, callerUserId, workspaceId, cancellationToken);

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

using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.BusinessEntities.AuthenticationService.V1.UserManagement;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.License.V1;
using ACS.Models.Request.V1.AuthenticationService.ManageUser;
using ACS.Models.Response;
using ACS.Models.Response.V1.AuthenticationService.Avatar;
using ACS.Models.Response.V1.AuthenticationService.UserAccess;
using ACS.Models.Response.V1.AuthenticationService.UserManagement;
using ACS.Models.Response.V1.AuthenticationService.UserRole;
using ACS.Service.V1.Interfaces;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Globalization;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class UserManagementService(ILicenseManager licenseManager, IMinioService minioService) : Service.Service(licenseManager), IUserManagementService
    {
        private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".webp"];
        const int maxFileSize = 10 * 1024 * 1024;

        public IUserManagementDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.UserManagementDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserManagementService.")
                };
            }
        }

        public async Task<GetUsersResponse> GetUsersAsync(int workspaceId, int callerUserId, int userId = 0, string? search = null, int statusId = 0, int limit = 50, int offset = 0, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetUsersAsync(workspaceId, callerUserId, userId, search, statusId, limit, offset, cancellationToken);
            return new()
            {
                Success = db.Success,
                Total   = db.Total,
                Data    = [.. db.Data.Select(MapUser)]
            };
        }


        public async Task<GetUserResponse> GetUserByIdAsync(int userId, int workspaceId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetUserByIdAsync(userId, workspaceId, cancellationToken);
            return new GetUserResponse
            {
                Success   = db.Success,
                Error   = db.Message,
                ErrorCode = db.Success ? null : "USER_NOT_FOUND",
                Data      = db.Data is null ? null : MapUser(db.Data)
            };
        }

        public async Task<CreateUserResponse> CreateUserAsync(int callerUserId, int workspaceId, CreateUserRequest request, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].CreateUserAsync(
                createdBy: callerUserId,
                workspaceId: workspaceId,
                username: request.Username,
                email: request.Email,
                password: request.Password,
                fullName: request.FullName,
                phoneNumber: request.PhoneNumber,
                department: request.Department,
                jobTitle: request.JobTitle,
                timezone: request.Timezone,
                language: request.Language,
                userStatusId: request.UserStatusId,
                avatarUrl: request.AvatarUrl,
                notificationPrefsJson: request.NotificationPreferences,
                cancellationToken: cancellationToken);
            return new CreateUserResponse
            {
                Success   = db.Success,
                Error   = db.Message,
                ErrorCode = db.ErrorCode,
                Errors    = db.Errors,
                Warnings  = db.Warnings,
                RequestId= requestId,
                Data      = db.Data is null ? null : new CreateUserDataResponse
                {
                    UserId         = db.Data.UserId,
                    CurrentUsers   = db.Data.CurrentUsers,
                    MaxUsers       = db.Data.MaxUsers,
                    RemainingSlots = db.Data.RemainingSlots
                }
            };
        }

        public async Task<UserOperationResponse> UpdateUserAsync(int callerUserId, int userId, int workspaceId, UpdateUserRequest request, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].UpdateUserAsync(
                updatedBy: callerUserId,
                userId: userId,
                workspaceId: workspaceId,
                fullName: request.FullName,
                phoneNumber: request.PhoneNumber,
                department: request.Department,
                jobTitle: request.JobTitle,
                timezone: request.Timezone,
                language: request.Language,
                avatarUrl: request.AvatarUrl,
                notificationPrefsJson: request.NotificationPreferences,
                cancellationToken: cancellationToken);
            return MapOperation(db);
        }

        

        public async Task<UserOperationResponse> SetUserStatusAsync(int callerUserId, int userId, int workspaceId, SetUserStatusRequest request, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].SetUserStatusAsync( callerUserId, userId, workspaceId, request.StatusCode, request.Reason, cancellationToken);
            return MapOperation(db);
        }

        public async Task<UserOperationResponse> DeleteUserAsync(int callerUserId, int userId, int workspaceId, DeleteUserRequest request, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].DeleteUserAsync(
                callerUserId, userId, workspaceId, request.Reason, cancellationToken);

            return MapOperation(db);
        }

        public async Task<UserOperationResponse> ResetPasswordAsync(int callerUserId, int userId, int workspaceId, ResetPasswordRequest request, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {

            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].ResetUserPasswordAsync(
                callerUserId, userId, workspaceId, request.NewPassword, request.Reason, cancellationToken);

            return MapOperation(db);
        }


        public async Task<UserActivityResponse> GetUserActivityAsync(
            int userId, 
            int workspaceId, 
            int callerUserId = 0, 
            int limit = 50, 
            int offset = 0, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetUserActivityAsync(
                userId, workspaceId, callerUserId, limit, offset, cancellationToken);

            return new UserActivityResponse
            {
                Success = db.Success,
                Total   = db.Total,
                Data    = [.. db.Data.Select(a => new ActivityLogResponse
                {
                    LogId           = a.LogId,
                    Action          = a.Action,
                    EntityType      = a.EntityType,
                    Description     = a.Description,
                    ResourceName    = a.ResourceName,
                    ResourcePath    = a.ResourcePath,
                    RequestId       = a.RequestId,
                    IpAddress       = a.IpAddress,
                    UserAgent       = a.UserAgent,
                    PerformedBy     = a.PerformedBy,
                    PerformedByName = a.PerformedByName,
                    OldValues       = a.OldValues,
                    NewValues       = a.NewValues,
                    ProjectId       = a.ProjectId,
                    ProjectAreaId   = a.ProjectAreaId,
                    ZoneId          = a.ZoneId,
                    AccessPointId   = a.AccessPointId,
                    Latitude        = a.Latitude,
                    Longitude       = a.Longitude,
                    CreatedAt       = a.CreatedAt,
                })]
            };
        }

        public async Task<UserOperationResponse> AssignRoleAsync(int callerUserId, int userId, int workspaceId, AssignRoleRequest request, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].AssignRoleAsync(
                callerUserId, userId, workspaceId, request.RoleId, cancellationToken);

            return MapOperation(db);
        }


        public async Task<UserOperationResponse> RemoveRoleAsync(int callerUserId, int userId, int workspaceId, int roleId, string? ipAddress = null, string? deviceInfo = null, string? agent = null, string? requestId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].RemoveRoleAsync(
                callerUserId, userId, workspaceId, roleId, cancellationToken);

            return MapOperation(db);
        }


        public async Task<CanManageUserResponse> CanManageUserAsync(int managerUserId, int targetUserId, int workspaceId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var canManage = await this[license?.DB!].CanManageUserAsync(
                managerUserId, targetUserId, workspaceId, cancellationToken);

            return new CanManageUserResponse { Success = true, CanManage = canManage };
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="callerUserId"></param>
        /// <param name="user"></param>
        /// <param name="avatar"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
       public async Task<UpdateAvatarResponse> UpdateAvatar(
            int workspaceId,
            int callerUserId,
            int user,
            IFormFile? avatar,
            string? ipAddress,
            string? deviceInfo,
            string? agent,
            string? requestId,
            double? latitude,
            double? longitude,
            CancellationToken cancellationToken = default)
        {
            if (avatar== null || !IsValidImageFile(avatar))
            {
                return new()
                {
                    Success = false,
                    Error = "Invalid vehicle image. File must be JPG, PNG, GIF, BMP, or WEBP and less than 10MB",
                    ErrorCode = "UNSUPPORTED_EXTENSION",
                    RequestId = requestId
                };
            }
            var license = this.LicenseManager.GetLicense();
            var ext = Path.GetExtension(avatar.FileName).ToLowerInvariant();
            var filename = $"{user}{ext}";
            using var carStream = avatar.OpenReadStream();
            var path = await minioService.UploadImageAsync(carStream,
                    workspaceId.ToString(),
                    filename,
                     avatar.ContentType, cancellationToken);
            var db = await this[license?.DB!].UpdateUserAvatarAsync(workspaceId, user, callerUserId, path, ipAddress, deviceInfo, agent, requestId, latitude??0, longitude??0, cancellationToken);
            if (!db.Success)
            {
                return new()
                {
                    Success = false,
                    Error = db.Message,
                    ErrorCode = db.ErrorCode,
                    RequestId = requestId
                };
            }
            return new() { Success= true, Error= null, ErrorCode= null, RequestId= requestId, Path= path };
        }

        public async Task<GetManageableUsersResponse> GetManageableUsersAsync(int managerUserId, int workspaceId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetManageableUsersAsync(managerUserId, workspaceId, cancellationToken);
            return new GetManageableUsersResponse
            {
                Success = true,
                Data    = [.. db.Select(u => new ManageableUserResponse
                {
                    UserId          = u.UserId,
                    FullName        = u.FullName,
                    Email           = u.Email,
                    Username        = u.Username,
                    AvatarUrl       = u.AvatarUrl,
                    ScopeTypeCode   = u.ScopeTypeCode,
                    ResourceId      = u.ResourceId,
                    AccessLevelCode = u.AccessLevelCode,
                    IsAdminScope    = u.IsAdminScope
                })]
            };
        }

        /// <summary>
        /// Transfer management of a user from one manager to another. Validates that caller can manage both users and that new manager has capacity in their admin scope. Idempotent.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="actorUserId"></param>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="agent"></param>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TransferUserManagerResponse> TransferUserManagerAsync(
            int workspaceId,
            int actorUserId,
            TransferUserManagerRequest request,
            string? ipAddress,
            string? deviceInfo,
            string? agent,
            string? requestId,
            CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            if (!double.TryParse(request.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude  = 0;
            if (!double.TryParse(request.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;

            var db = await this[license?.DB!].TransferUserManagerAsync(
                workspaceId,
                actorUserId,
                request.ManagedUserId,
                request.NewManagedId,
                request.Notes,
                ipAddress,
                deviceInfo,
                agent,
                requestId,
                _latitude,
                _longitude,
                cancellationToken);
            return new ()
            {
                Success = db.Success,
                Error = db.Message,
                ErrorCode = db.ErrorCode,
                RequestId = requestId,
                ManagedUserId = db.ManagedUserId,
                NewManagedId = db.NewManagedId,
                NewManagedName = db.NewManagedName
            };
        }



            

        
        private static UserResponse MapUser(UserEntity u) => new()
        {
            UserId                  = u.UserId,
            WorkspaceId             = u.WorkspaceId,
            Username                = u.Username,
            Email                   = u.Email,
            FullName                = u.FullName,
            PhoneNumber             = u.PhoneNumber,
            AvatarUrl               = u.AvatarUrl,
            Department              = u.Department,
            JobTitle                = u.JobTitle,
            Timezone                = u.Timezone,
            Language                = u.Language,
            MfaEnabled              = u.MfaEnabled,
            NotificationPreferences = u.NotificationPreferences,
            LastLogin               = u.LastLogin,
            UserStatusCode          = u.UserStatusCode,
            UserStatusName          = u.UserStatusName,
            FailedLoginAttempts     = u.FailedLoginAttempts,
            LockedUntil             = u.LockedUntil,
            PasswordChangedAt       = u.PasswordChangedAt,
            CreatedAt               = u.CreatedAt,
            UpdatedAt               = u.UpdatedAt,
            ScopeTypeCode           = u.ScopeTypeCode,
            ManagerUerId            = u.ManagerUerId,
            Manager                 = u.Manager == null ? null : new ManagerUserResponse
            {
                AssignedAt        = u.Manager.AssignedAt,
                ManagerAvatarUrl  = u.Manager.ManagerAvatarUrl,
                ManagerEmail      = u.Manager.ManagerEmail,
                ManagerFullName   = u.Manager.ManagerFullName,
                ManagerUserId     = u.Manager.ManagerUserId,
                ManagerUsername   = u.Manager.ManagerUsername,
                Notes             = u.Manager.Notes
            },
            Roles = [.. u.Roles.Select(r => new RoleSummaryResponse
            {
                UserRoleId = r.UserRoleId,
                RoleNames  = r.RoleNames
            })],
            AccessRules = u.AccessRules?.Select(a => new AccessRuleSummaryResponse
            {
                UserAccessId    = a.UserAccessId,
                ScopeCode       = a.ScopeCode,
                ScopeLevel      = a.ScopeLevel,
                AccessLevelCode = a.AccessLevelCode,
                IsAdminScope    = a.IsAdminScope,
                IsInherited     = a.IsInherited,
                ProjectId       = a.ProjectId,
                ProjectAreaId   = a.ProjectAreaId,
                ZoneId          = a.ZoneId,
                AccessPointId   = a.AccessPointId,
                ResourceName    = a.ResourceName,
                ExpiresAt       = a.ExpiresAt,
                GrantedAt       = a.GrantedAt,
                ScopeTypeCode   = a.ScopeTypeCode,
                ScopeTypeId     = a.ScopeTypeId
            }).ToList()
        };

        

        private static UserOperationResponse MapOperation(UserOperationResultEntity db) => new()
        {
            Success   = db.Success,
            Error   = db.Message,
            ErrorCode = db.ErrorCode,
            Data      = db.Data
        };


        private static bool IsValidImageFile(IFormFile? file)
        {
            if (file == null) return true;

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext.ToLower()))
                return false;

            if (file.Length > maxFileSize)
                return false;

            return true;
        }

    }
}

using ACS.Service.V1.Interfaces;
using ACS.Models.Response.V1.SetupService;
using ACS.BusinessEntities.SetupService.V1;
using ACS.BusinessEntities.SubscriberService.V1;
using ACS.Models.Response.V1.MasterService.County;
using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.BusinessEntities.MasterService.V1.County;
using ACS.BusinessEntities.ParkingService.V1.Entry;
using ACS.Models.Response.V1.ParkingService.Session;
using ACS.Parking.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.PlateState;
using ACS.BusinessEntities.MasterService.V1.PlateState;
using ACS.Models.Response.V1.MasterService.PlateCategory;
using ACS.BusinessEntities.MasterService.V1.PlateCategory;
using ACS.Models.Response.V1.AuthenticationService.Account;
using ACS.BusinessEntities.AuthenticationService.V1.Authentication;

namespace ACS.Parking.WebService.Services.V1.Services
{
    public class BaseService(ILicenseManager licenseManager) : Service.Service(licenseManager), IBaseService
    {
        public SessionsResponse MapSessionsToResponseList(EntrySessionsEntity entity)
        {
            if (entity == null) return new() { Success = false, Error = "No data received" };
            if (!entity.Success) return new() { Success = false, Error = entity.Message, ErrorCode = entity.ErrorCode };
            var sessions = entity.Sessions?
                .Select(session => MapSessionDataToResponse(session))
                .Where(session => session != null)
                .Select(session => session!)
                .ToList();
            var pagination = entity.Pagination != null ? new PaginationInfoResponse
            {
                Total = entity.Pagination.Total,
                Skip = entity.Pagination.Skip,
                Take = entity.Pagination.Take,
                HasMore = entity.Pagination.HasMore
            } : null;

            return new()
            {
                Success = true,
                Pagination = pagination,
                Sessions = sessions
            };
        }

        private EntrySessionResponse MapToResponse(EntrySessionEntity entity)
        {
            if (entity == null)
                return new EntrySessionResponse { Success = false, Error = "No data received" };

            if (!entity.Success)
            {
                return new EntrySessionResponse
                {
                    Success = false,
                    Error = entity.Message,
                    ErrorCode = entity.ErrorCode,
                    SystemError = entity.SystemError,
                    Timestamp = DateTime.UtcNow
                };
            }

            return new EntrySessionResponse
            {
                Success = true,
                Error = entity.Message,
                ErrorCode = entity.ErrorCode,
                Data = MapSessionDataToResponse(entity.Data),
                Metadata = MapMetadataToResponse(entity.Metadata),
                SystemError = entity.SystemError,
                Timestamp = entity.Timestamp ?? DateTime.UtcNow,
                RequestId = entity.RequestId
            };
        }


        public ParkingSessionDataResponse? MapSessionDataToResponse(SessionDataEntity? entity) =>
            entity == null ? null : new()
            {
                ParkingSession = MapParkingSession(entity.ParkingSession),
                AccessPoint = MapAccessPoint(entity.AccessPoint),
                Project = MapProject(entity.Project),
                AreaZone = MapAreaZone(entity.AreaZone),
                Subscriber = MapSubscriber(entity.Subscriber),
                Country = MapCountry(entity.Country),
                PlateState = MapPlateState(entity.PlateState),
                PlateCategory = MapPlateCategory(entity.PlateCategory),
                User = MapUser(entity.User),
                UserSession = MapUserSession(entity.UserSession),
                Workspace = MapWorkspace(entity.Workspace)
            };


        public ParkingSessionResponse? MapParkingSession(ParkingSessionEntity? entity) =>
            entity == null ? null : new()
            {
                ParkingSessionId = entity.ParkingSessionId,
                EntryPlateCode = entity.EntryPlateCode,
                EntryPlateNumber = entity.EntryPlateNumber,
                EntryFullPlate = entity.EntryFullPlate,
                EntryTime = entity.EntryTime,
                EntryLatitude = entity.EntryLatitude,
                EntryLongitude = entity.EntryLongitude,
                EntryAnprTransId = entity.EntryAnprTransId,
                EntryCameraCaptureUrl = entity.EntryCameraCaptureUrl,
                QrCode = entity.QrCode,
                QrCodeExpiry = entity.QrCodeExpiry,
                Status = entity.Status,
                CurrentFullPlate = entity.CurrentFullPlate,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                SessionCode = entity.SessionCode
            };


        public AccessPointResponse? MapAccessPoint(AccessPointEntity? entity) =>
            entity == null ? null : new()
            {
                Success = true,
                AccessPointId = entity.AccessPointId,
                AccessPointName = entity.AccessPointName,
                AccessPointTypeCode = entity.AccessPointTypeCode,
                AccessPointTypeNames = entity.AccessPointTypeNames,
                PositionX = entity.PositionX,
                PositionY = entity.PositionY,
                OrientationDegrees = entity.OrientationDegrees,
                AccessPointLatitude = entity.AccessPointLatitude,
                AccessPointLongitude = entity.AccessPointLongitude,
                ZoneId = entity.ZoneId,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

        public ProjectResponse? MapProject(ProjectEntity? entity) =>
            entity == null ? null : new()
            {
                Success = true,
                ProjectId = entity.ProjectId,
                ProjectNames = entity.ProjectNames,
                ProjectDescription = entity.ProjectDescription,
                ProjectAddress = entity.ProjectAddress,
                ProjectCity = entity.ProjectCity,
                ProjectState = entity.ProjectState,
                CountryId = entity.CountryId,
                PostalCode = entity.PostalCode,
                ProjectLatitude = entity.ProjectLatitude,
                ProjectLongitude = entity.ProjectLongitude,
                Timezone = entity.Timezone,
                ProjectIsPublic = entity.ProjectIsPublic,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

        public AreaZoneResponse? MapAreaZone(AreaZoneEntity? entity) =>
            entity == null ? null : new()
            {
                Success = true,
                ZoneId = entity.ZoneId,
                ZoneCode = entity.ZoneCode,
                ZoneNames = entity.ZoneNames,
                ProjectId = entity.ProjectId,
                ProjectAreaId = entity.ProjectAreaId,
                ParentZoneId = entity.ParentZoneId,
                ZoneTypeId = entity.ZoneTypeId,
                FloorNumber = entity.FloorNumber,
                TotalSpots = entity.TotalSpots,
                AvailableSpots = entity.AvailableSpots,
                CenterLatitude = entity.CenterLatitude,
                CenterLongitude = entity.CenterLongitude,
                AccessLevelId = entity.AccessLevelId,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

        public SubscriberResponse? MapSubscriber(SubscriberEntity? entity) =>
            entity == null ? null : new()
            {
                SubscriberId = entity.SubscriberId,
                ProjectId = entity.ProjectId,
                SubscriberType = entity.SubscriberType,
                Name = entity.Name,
                ContactEmail = entity.ContactEmail,
                ContactPhone = entity.ContactPhone,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };


        public CountryResponse? MapCountry(CountryEntity? entity) =>
            entity == null ? null : new()
            {
                CountryId = entity.CountryId,
                CountryCode = entity.CountryCode,
                CountryNames = entity.CountryNames,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                Alphabets = entity.Alphabets,
                Digits = entity.Digits,
                PatternRegex = entity.PatternRegex,
                PatternDescription = entity.PatternDescription,
                PlateConfigCreatedAt = entity.PlateConfigCreatedAt,
                PlateConfigUpdatedAt = entity.PlateConfigUpdatedAt
            };


        public PlateStateResponse? MapPlateState(PlateStateEntity? entity) =>
            entity == null ? null : new()
            {
                PlateStateId = entity.PlateStateId,
                PlateStateName = entity.PlateStateName,
                CountryId = entity.CountryId,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };

        public PlateCategoryResponse? MapPlateCategory(PlateCategoryEntity? entity) =>
            entity == null ? null : new()
            {
                PlateCategoryId = entity.PlateCategoryId,
                CategoryCode = entity.CategoryCode,
                CategoryNames = entity.CategoryNames,
                CategoryDescriptions = entity.CategoryDescriptions,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };

        public UserProfileResponse? MapUser(UserProfileEntity? entity) =>
            entity == null ? null : new()
            {
                UserId = entity.UserId,
                Username = entity.Username,
                Email = entity.Email,
                FullName = entity.FullName,
                AvatarUrl = entity.AvatarUrl,
                Department = entity.Department,
                JobTitle = entity.JobTitle,
                Timezone = entity.Timezone,
                Language = entity.Language,
                LastLogin = entity.LastLogin,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                WorkspaceId = entity.WorkspaceId,
                WorkspaceName = entity.WorkspaceName
            };


        public UserSessionResponse? MapUserSession(UserSessionEntity? entity) =>
            entity == null ? null : new()
            {
                SessionId = entity.SessionId,
                UserId = entity.UserId,
                WorkspaceId = entity.WorkspaceId,
                TokenExpiresAt = entity.TokenExpiresAt,
                RefreshTokenExpiresAt = entity.RefreshTokenExpiresAt,
                IpAddress = entity.IpAddress,
                UserAgent = entity.UserAgent,
                DeviceInfo = entity.DeviceInfo,
                ClientId = entity.ClientId,
                ClientVersion = entity.ClientVersion,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                LastAccessedAt = entity.LastAccessedAt,
                LogoutAt = entity.LogoutAt,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                FailedRefreshAttempts = entity.FailedRefreshAttempts
            };


        public WorkspaceResponse? MapWorkspace(WorkspaceEntity? entity) =>
            entity == null ? null : new()
            {
                WorkspaceId = entity.WorkspaceId,
                WorkspaceCode = entity.WorkspaceCode,
                WorkspaceName = entity.WorkspaceName,
                Description = entity.Description,
                LicenseStatus = entity.LicenseStatus,
                MaxHardware = entity.MaxHardware,
                MaxUsers = entity.MaxUsers,
                MaxParkingSpots = entity.MaxParkingSpots,
                MaxVehicleRecords = entity.MaxVehicleRecords,
                CurrentHardware = entity.CurrentHardware,
                CurrentUsers = entity.CurrentUsers,
                CurrentParkingSpots = entity.CurrentParkingSpots,
                CurrentVehicleRecords = entity.CurrentVehicleRecords,
                Timezone = entity.Timezone,
                Language = entity.Language,
                IsActive = entity.IsActive,
                ContractStartDate = entity.ContractStartDate,
                ContractEndDate = entity.ContractEndDate,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

        public MetadataResponse? MapMetadataToResponse(MetadataEntity? entity) =>
            entity == null ? null : new()
            {
                Timestamp = entity.Timestamp,
                RequestId = entity.RequestId,
                SessionId = entity.SessionId,
                EntryTime = entity.EntryTime,
                VehiclePlate = entity.VehiclePlate,
                AccessPointId = entity.AccessPointId,
                ProjectId = entity.ProjectId,
                AreaZoneId = entity.AreaZoneId,
                WorkspaceId = entity.WorkspaceId,
                SubscriberId = entity.SubscriberId,
                QrCode = entity.QrCode,
                QrCodeExpiry = entity.QrCodeExpiry,
                AuditLogId = entity.AuditLogId
            };

    }
}

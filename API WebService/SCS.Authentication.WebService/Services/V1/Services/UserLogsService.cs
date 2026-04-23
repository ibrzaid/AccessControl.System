using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.AuthenticationService.V1;
using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.AuthenticationService.UserLogs;
using ACS.BusinessEntities.AuthenticationService.V1.UserLogs;

namespace ACS.Authentication.WebService.Services.V1.Services
{
    public class UserLogsService(ILicenseManager licenseManager) : Service.Service(licenseManager), IUserLogsService
    {
        public IUserLogsDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.AuthenticationService.V1.UserLogsDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in UserLogsService.")
                };
            }
        }

        public async Task<UserLogsResponse> GetUserLogsAsync(
            int workspaceId, 
            int userId, int? targetUser, 
            string? action, 
            string? entityType, 
            DateTime fromDate, 
            DateTime toDate, 
            string requestId, 
            int limit = 50, 
            int page = 0,
            string? orderBy = "DESC",
            CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetUserLogsAsync(workspaceId, userId, targetUser, action, entityType, fromDate, toDate, limit, page, orderBy, cancellationToken);
            return new()
            {
                Success = db.Success,
                ErrorCode = db.ErrorCode,
                RequestId = requestId,
                Error = db.Message,
                Timezone = db.Timezone,
                Total = db.Total,
                Data = MapToDatumResponseList(db.Data),
                Analytics = MapToAnalyticsResponse(db.Analytics)
            };
        }

        private static List<UserLogsDatumResponse>? MapToDatumResponseList(List<DatumEntity>? source) =>
            (source == null || source.Count == 0) ? null
            : [.. source.Select(MapToDatumResponse)
                .Where(x => x != null).Select(x => x!)];

        private static UserLogsDatumResponse? MapToDatumResponse(DatumEntity source) =>
            (source == null) ? null
            : new UserLogsDatumResponse
            {
                Id = source.Id,
                Action = source.Action,
                UserId = source.UserId,
                UserIp = source.UserIp,
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                UserName = source.UserName,
                CreatedAt = source.CreatedAt,
                NewValues = source.NewValues,
                OldValues = MapToOldValuesResponse(source.OldValues),
                ProjectId = source.ProjectId,
                RequestId = source.RequestId,
                UserAgent = source.UserAgent,
                Description = source.Description,
                EntityType = source.EntityType,
                AreaZoneId = source.AreaZoneId,
                ResourcePath = source.ResourcePath,
                AccessPointId = source.AccessPointId,
                ProjectAreaId = source.ProjectAreaId
            };


        private static UserLogsOldValuesResponse? MapToOldValuesResponse(OldValuesEntity? source) =>
            (source == null) ? null
            : new UserLogsOldValuesResponse
            {
                UserId = source.UserId,
                Username = source.Username
            };


        private static UserLogsAnalyticsResponse? MapToAnalyticsResponse(AnalyticsEntity? source) =>
            (source == null) ? null
            : new UserLogsAnalyticsResponse
            {
                Login = MapToLoginResponse(source.Login),
                Network = MapToNetworkResponse(source.Network),
                Overview = MapToOverviewResponse(source.Overview),
                Sessions = MapToSessionsResponse(source.Sessions),
                TopPaths = MapToTopPathResponseList(source.TopPaths),
                Management = MapToManagementResponse(source.Management),
                ActivityByDay = MapToActivityByDayResponseList(source.ActivityByDay),
                ActivityByHour = MapToActivityByHourResponseList(source.ActivityByHour),
                ActionsBreakdown = MapToActionsBreakdownResponseList(source.ActionsBreakdown)
            };



        private static UserLogsLoginResponse? MapToLoginResponse(LoginEntity? source) =>
            (source == null) ? null
            : new UserLogsLoginResponse
            {
                LastLogin = source.LastLogin,
                FirstLogin = source.FirstLogin,
                TotalLogins = source.TotalLogins,
                FailedLogins = source.FailedLogins,
                TotalLogouts = source.TotalLogouts,
                TotalRefreshes = source.TotalRefreshes,
                LastFailedLogin = source.LastFailedLogin
            };


        private static UserLogsNetworkResponse? MapToNetworkResponse(NetworkEntity? source) =>
             (source == null) ? null
            : new UserLogsNetworkResponse
            {
                IpList = source.IpList,
                UniqueIps = source.UniqueIps
            };



        private static UserLogsOverviewResponse? MapToOverviewResponse(OverviewEntity? source) =>
             (source == null) ? null
            : new UserLogsOverviewResponse
            {
                ActiveDays = source.ActiveDays,
                TotalEvents = source.TotalEvents,
                LastActivity = source.LastActivity,
                FirstActivity = source.FirstActivity
            };



        private static UserLogsSessionsResponse? MapToSessionsResponse(SessionsEntity? source) =>
             (source == null) ? null
             : new UserLogsSessionsResponse
             {
                 Devices = MapToDeviceResponseList(source.Devices),
                 UniqueIps = source.UniqueIps,
                 FirstSession = source.FirstSession,
                 LatestSession = source.LatestSession,
                 TotalSessions = source.TotalSessions,
                 UniqueClients = source.UniqueClients,
                 ActiveSessions = source.ActiveSessions,
                 ClosedSessions = source.ClosedSessions,
                 LastSessionActivity = source.LastSessionActivity,
                 TotalFailedRefreshes = source.TotalFailedRefreshes
             };
        


        private static List<UserLogsDeviceResponse>? MapToDeviceResponseList(List<DeviceEntity>? source) =>
             (source == null || source.Count == 0) ? null
             : [.. source.Select(MapToDeviceResponse).Where(x => x != null).Select(x => x!)];


        private static UserLogsDeviceResponse? MapToDeviceResponse(DeviceEntity source) =>
             (source == null) ? null
             : new UserLogsDeviceResponse
             {
                 Latitude = source.Latitude,
                 ClientId = source.ClientId,
                 IsActive = source.IsActive,
                 LogoutAt = source.LogoutAt,
                 Longitude = source.Longitude,
                 CreatedAt = source.CreatedAt,
                 IpAddress = source.IpAddress,
                 DeviceInfo = MapToDeviceInfoResponse(source.DeviceInfo),
                 ClientVersion = source.ClientVersion,
                 LastAccessedAt = source.LastAccessedAt,
                 FailedRefreshAttempts = source.FailedRefreshAttempts
             };



        private static UserLogsDeviceInfoResponse? MapToDeviceInfoResponse(DeviceInfoEntity? source) =>
             (source == null) ? null
             : new UserLogsDeviceInfoResponse
             {
                 Os = source.Os,
                 Browser = source.Browser,
                 Version = source.Version
             };



        private static UserLogsManagementResponse? MapToManagementResponse(ManagementEntity? source) =>
             (source == null) ? null
             : new UserLogsManagementResponse
             {
                 TotalGrants = source.TotalGrants,
                 TotalCreates = source.TotalCreates,
                 TotalDeletes = source.TotalDeletes,
                 TotalUpdates = source.TotalUpdates,
                 TotalAssignments = source.TotalAssignments,
                 TotalStatusChanges = source.TotalStatusChanges,
                 TotalPasswordResets = source.TotalPasswordResets
             };
        


        private static List<UserLogsTopPathResponse>? MapToTopPathResponseList(List<TopPathEntity>? source) =>
             (source == null || source.Count == 0) ? null
             : [.. source.Select(MapToTopPathResponse).Where(x => x != null).Select(x => x!)];


        private static UserLogsTopPathResponse? MapToTopPathResponse(TopPathEntity source) =>
             (source == null) ? null
             : new UserLogsTopPathResponse
             {
                 Path = source.Path,
                 Count = source.Count
             };
        


        private static List<UserLogsActivityByDayResponse>? MapToActivityByDayResponseList(List<ActivityByDayEntity>? source)=>
             (source == null || source.Count == 0) ? null 
            : [.. source.Select(MapToActivityByDayResponse).Where(x => x != null).Select(x => x!)];



        private static UserLogsActivityByDayResponse? MapToActivityByDayResponse(ActivityByDayEntity source) =>
             (source == null) ? null
             : new UserLogsActivityByDayResponse
             {
                 Date = source.Date,
                 Count = source.Count
             };
        


        private static List<UserLogsActivityByHourResponse>? MapToActivityByHourResponseList(List<ActivityByHourEntity>? source)=>
             (source == null || source.Count == 0) ? null
             : [.. source.Select(MapToActivityByHourResponse).Where(x => x != null).Select(x => x!)];


        private static UserLogsActivityByHourResponse? MapToActivityByHourResponse(ActivityByHourEntity source) =>
             (source == null) ? null
             : new UserLogsActivityByHourResponse
             {
                 Hour = source.Hour,
                 Count = source.Count
             };
        


        private static List<UserLogsActionsBreakdownResponse>? MapToActionsBreakdownResponseList(List<ActionsBreakdownEntity>? source) =>
             (source == null || source.Count == 0) ? null
             : [.. source.Select(MapToActionsBreakdownResponse).Where(x => x != null).Select(x => x!)];


        private static UserLogsActionsBreakdownResponse? MapToActionsBreakdownResponse(ActionsBreakdownEntity source) =>
             (source == null) ? null
             : new UserLogsActionsBreakdownResponse
            {
                Count = source.Count,
                Action = source.Action,
                EntityType = source.EntityType
            };
        

    }
}

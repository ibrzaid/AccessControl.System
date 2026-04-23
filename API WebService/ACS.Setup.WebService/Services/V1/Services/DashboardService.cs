using ACS.Helper;
using ACS.Background;
using ACS.License.V1;
using System.Text.Json;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.SetupService.V1;
using ACS.Setup.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.SetupService.Dashboard;

namespace ACS.Setup.WebService.Services.V1.Services
{
    public class DashboardService(ILicenseManager licenseManager, IBackgroundTaskQueue backgroundTaskQueue, INotificationService notificationService, ILogger<DashboardService> logger) : Service.Service(licenseManager), IDashboardService
    {
        private IDashboardDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.SetupService.V1.DashboardDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in DashoardService.")
                };
            }
        }

        public void StartNotify()
        {
            backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                try
                {
                    var license = this.LicenseManager.GetLicense();
                    this[license?.DB!].DashboardChanged+=(object? sender, JsonElement e) =>
                    {
                        var ev = e.Str("event");
                        var workspaceId = e.IntN("workspace_id");
                        var projectId = e.IntN("project_id");
                        var userId = e.IntN("user_id");
                        var data = e.TryGetProperty("data", out var d) ? d : default;
                        if (workspaceId== null) return;
                        switch (ev)
                        {
                            case "hardware_status_changed":
                                object payload = new
                                {
                                    hardware_id = data.IntN("hardware_id"),
                                    new_status = data.Str("new_status"),
                                    old_status = data.Str("old_status"),
                                    last_activity = data.Str("last_activity"),
                                    access_point_id = data.IntN("access_point_id"),
                                    workspace_id = workspaceId,
                                    project_id = projectId,
                                };
                                notificationService.SendToWorkspaceGroup(ev, workspaceId.ToString()!, payload);
                                if(projectId.HasValue) notificationService.SendToProjectGroup(ev, workspaceId?.ToString()!, projectId.Value.ToString(), payload);
                                break;

                            case "blacklist_hit_recorded":
                                payload = new
                                {
                                    hit_id = data.LngN("hit_id"),
                                    detection_id = data.LngN("detection_id"),
                                    plate = data.Str("plate"),
                                    plate_code = data.Str("plate_code"),
                                    plate_number = data.Str("plate_number"),
                                    hit_time = data.Str("hit_time"),
                                    access_point_id = data.IntN("access_point_id"),
                                    workspace_id = workspaceId,
                                    project_id = projectId,
                                };
                                notificationService.SendToWorkspaceGroup(ev, workspaceId.ToString()!, payload);
                                if (projectId.HasValue) notificationService.SendToProjectGroup(ev, workspaceId?.ToString()!, projectId.Value.ToString(), payload);
                                break;

                            case "hardware_summary_updated":
                                payload = new
                                {
                                    summary_date = data.Str("summary_date"),
                                    total_cameras = data.IntN("total_cameras"),
                                    online_cameras = data.IntN("online_cameras"),
                                    offline_cameras = data.IntN("offline_cameras"),
                                    warning_cameras = data.IntN("warning_cameras"),
                                    uptime_pct = data.DblN("uptime_pct"),
                                    cameras_with_errors = data.IntN("cameras_with_errors"),
                                    online_pct = data.DblN("online_pct"),
                                    workspace_id = workspaceId,
                                    project_id = projectId,
                                };
                                notificationService.SendToWorkspaceGroup(ev, workspaceId.ToString()!, payload);
                                if (projectId.HasValue) notificationService.SendToProjectGroup(ev, workspaceId.ToString()!, projectId.Value.ToString(), payload);
                                break;
                        }

                    }
                        ;
                    await this[license?.DB!].StartNotify(token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while listening to dashoard notification.");
                }
            });
        }

        public async Task<SetupDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDashboardAsync(wrokspace, user, cancellationToken);
            return new(
                Success: result.Success,
                Message: result.Message,
                ErrorCode: "",
                RequestId: requestId,
                GeneratedAt: result.GeneratedAt,
                Detail: result.Detail,
                UserContext: result.UserContext == null ? null : new(
                    UserId: result.UserContext.UserId,
                    FullName: result.UserContext.FullName,
                    Email: result.UserContext.Email,
                    AvatarUrl: result.UserContext.AvatarUrl,
                    Language: result.UserContext.Language,
                    Timezone: result.UserContext.Timezone,
                    LastLogin: result.UserContext.LastLogin,
                    WorkspaceId: result.UserContext.WorkspaceId,
                    WorkspaceName: result.UserContext.WorkspaceName,
                    WorkspaceCode: result.UserContext.WorkspaceCode,
                    ScopeTypeId: result.UserContext.ScopeTypeId,
                    ProjectIds: result.UserContext.ProjectIds,
                    Modules: result.UserContext.Modules == null ? null : new(
                          Anpr: result.UserContext.Modules.Anpr,
                          Parking: result.UserContext.Modules.Parking,
                          Gates: result.UserContext.Modules.Gates,
                          Prebooking: result.UserContext.Modules.Prebooking,
                          Blacklist: result.UserContext.Modules.Blacklist)
                    ),
                Cameras: result.Cameras == null ? null : new(
                    Online: result.Cameras.Online,
                    Offline: result.Cameras.Offline,
                    Warning: result.Cameras.Warning,
                    Total: result.Cameras.Total,
                    UptimePct: result.Cameras.UptimePct,
                    List: result.Cameras.List == null ? [] : [.. result.Cameras.List.Select(camera =>
                    new CameraListItemResponse(
                        HardwareId: camera.HardwareId,
                        Model: camera.Model,
                        Manufacturer: camera.Manufacturer,
                        Status: camera.Status,
                        AccessPoint: camera.AccessPoint,
                        LastActivity: camera.LastActivity,
                        DetectionCount: camera.DetectionCount,
                        ProjectId: camera.ProjectId,
                        ProjectAreaId: camera.ProjectAreaId,
                        ZoneId: camera.ZoneId
                        ))]
                    ),
                Projects: result.Projects == null ? [] : [.. result.Projects.Select( project => 
                new ProjectCardResponse(
                     ProjectId: project.ProjectId,
                     ProjectName: project.ProjectName,
                     ProjectNames: project.ProjectNames,
                     City: project.City,
                     CamerasOnline: project.CamerasOnline,
                     CamerasTotal: project.CamerasTotal,
                     DetectionsToday: project.DetectionsToday,  
                     ActiveSessions: project.ActiveSessions
                    ) )]
                );

        }
    }
}
using ACS.Background;
using ACS.Helper;
using ACS.Database.IDataAccess.ParkingService.V1;
using ACS.License.V1;
using ACS.Models.Response.V1.ParkingService.Dashboard;
using ACS.Models.Response.V1.WebhooksService.Dashboard;
using ACS.Parking.WebService.Services.V1.Interfaces;
using ACS.Service.V1.Interfaces;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Text.Json;

namespace ACS.Parking.WebService.Services.V1.Services
{
    public class DashboardService(ILicenseManager licenseManager, IBackgroundTaskQueue backgroundTaskQueue, INotificationService notificationService, ILogger<DashboardService> logger) : Service.Service(licenseManager), IDashboardService
    {
        private IDashboardDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.ParkingService.V1.DashboardDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
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
                        var signalREvent = ev == "session_overstay" ? "vehicle_exited" : ev;
                        if (workspaceId== null) return;
                        object payload = ev switch
                        {
                            "vehicle_entered" => new
                            {
                                parking_session_id = data.LngN("parking_session_id"),
                                plate = data.Str("plate"),
                                entry_time = data.Str("entry_time"),
                                project_id = projectId,
                                workspace_id = workspaceId,
                            },
                            "vehicle_exited" or "SessionOverstay" => new
                            {
                                parking_session_id = data.LngN("parking_session_id"),
                                plate = data.Str("plate"),
                                duration_minutes = data.IntN("duration_minutes"),
                                overstay = data.Bool("overstay") ?? ev == "SessionOverstay",
                                project_id = projectId,
                                workspace_id = workspaceId,
                            },
                            "session_cancelled" => new
                            {
                                parking_session_id = data.LngN("parking_session_id"),
                                plate = data.Str("plate"),
                                project_id = projectId,
                                workspace_id = workspaceId,
                            },
                            "payment_collected" => new
                            {
                                parking_session_id = data.LngN("parking_session_id"),
                                gross_amount = data.DblN("gross_amount"),
                                payment_method = data.Str("payment_method"),
                                project_id = projectId,
                                workspace_id = workspaceId,
                            },
                            _ => (object)new { workspace_id = workspaceId }
                        };
                        notificationService.SendToWorkspaceGroup(signalREvent ?? "", workspaceId.ToString()!, payload);
                        if (projectId.HasValue) notificationService.SendToProjectGroup(signalREvent?? "", workspaceId?.ToString()!, projectId.Value.ToString(), payload);
                    };
                    await this[license?.DB!].StartNotify(token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while listening to dashoard notification.");
                }
            });
        }

        public async Task<ParkingDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default)
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
                Module: result.Module,
                Enabled: result.Enabled,
                Parking: result.Parking == null ? null : new(
                    ActiveSessions: result.Parking.ActiveSessions,
                    RevenueToday: result.Parking.RevenueToday,
                    SessionsToday: result.Parking.SessionsToday,
                    SessionsClosedToday: result.Parking.SessionsClosedToday,
                    OverstayCount: result.Parking.OverstayCount,
                    AvgDurationMinutes: result.Parking.AvgDurationMinutes
                   ),
                ProjectsSessions: result.ProjectsSessions == null ?[] : [.. result.ProjectsSessions.Select(project => new ProjectSessionResponse(
                    ProjectId: project.ProjectId,
                    ActiveSessions: project.ActiveSessions
                    ))]
                );
        }
    }
}

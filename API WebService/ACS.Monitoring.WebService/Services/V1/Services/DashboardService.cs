using ACS.Background;
using ACS.Database.IDataAccess.MonitoringService.V1;
using ACS.License.V1;
using ACS.Models.Response.V1.MonitoringService.Dashboard;
using ACS.Monitoring.WebService.Services.V1.Interfaces;
using ACS.Service.V1.Interfaces;
using System.Net.NetworkInformation;
using System.Text.Json;
using ACS.Helper;

namespace ACS.Monitoring.WebService.Services.V1.Services
{
    public class DashboardService(ILicenseManager licenseManager, IBackgroundTaskQueue backgroundTaskQueue, INotificationService notificationService, ILogger<DashboardService> logger) : Service.Service(licenseManager), IDashboardService
    {
        private IDashboardDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.MonitoringService.V1.DashboardDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
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
                            case "incident_raised":
                                object payload = new
                                {
                                    incident_id = data.LngN("incident_id"),
                                    alert_title = data.Str("alert_title"),
                                    severity = data.Str("severity"),
                                    status = data.Str("status"),
                                    source = data.Str("source"),
                                    detected_at = data.Str("detected_at"),
                                    workspace_id = workspaceId,
                                    project_id = projectId,
                                };
                                notificationService.SendToWorkspaceGroup(ev, workspaceId.ToString()!, payload);
                                if (projectId.HasValue) notificationService.SendToProjectGroup(ev, workspaceId?.ToString()!, projectId.Value.ToString(), payload);
                                break;
                            case "incident_status_changed":
                                payload = new
                                {
                                    incident_id = data.LngN("incident_id"),
                                    status = data.Str("status"),
                                    old_status = data.Str("old_status"),
                                    resolved_at = data.Str("resolved_at"),
                                    workspace_id = workspaceId,
                                    project_id = projectId,
                                };
                                notificationService.SendToWorkspaceGroup(ev, workspaceId.ToString()!, payload);
                                if (projectId.HasValue) notificationService.SendToProjectGroup(ev, workspaceId?.ToString()!, projectId.Value.ToString(), payload);
                                break;
                        }
                    };
                    await this[license?.DB!].StartNotify(token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while listening to dashoard notification.");
                }
            });
        }

        public async Task<MonitoringDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default)
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
                Incidents: result.Incidents == null ? null : new(
                    OpenCount: result.Incidents.OpenCount,
                    CriticalCount: result.Incidents.CriticalCount,
                    Recent: result.Incidents.Recent == null ?[] : [..result.Incidents.Recent.Select(recent => 
                    new IncidentItemResponse(
                        IncidentId: recent.IncidentId,
                        AlertTitle: recent.AlertTitle,
                        Severity: recent.Severity,
                        Status: recent.Status,
                        Source: recent.Source,
                        DetectedAt: recent.DetectedAt
                        ))],    
                    BySeverity: result.Incidents.BySeverity == null ?[] : [.. result.Incidents.BySeverity.Select(severity => 
                    new IncidentSeverityResponse(
                        Severity: severity.Severity,
                        Count: severity.Count
                        ))]
                    )
                );
        }
    }
}
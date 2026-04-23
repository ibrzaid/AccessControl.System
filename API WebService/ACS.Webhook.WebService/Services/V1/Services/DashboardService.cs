using ACS.Helper;
using ACS.Background;
using ACS.License.V1;
using System.Text.Json;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.WebhooksService.V1;
using ACS.Webhook.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.WebhooksService.Dashboard;

namespace ACS.Webhook.WebService.Services.V1.Services
{
    public class DashboardService(ILicenseManager licenseManager, IBackgroundTaskQueue backgroundTaskQueue, INotificationService notificationService, ILogger<DashboardService> logger) : Service.Service(licenseManager), IDashboardService
    {
        private IDashboardDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.WebhooksService.V1.DashboardDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
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
                            case "webhook_delivered":
                                notificationService.SendToAdminsGroup(ev, workspaceId?.ToString()??"", new
                                {
                                    log_id = data.LngN("log_id"),
                                    config_name = data.Str("config_name"),
                                    event_type = data.Str("event_type"),
                                    delivered_at = data.Str("delivered_at"),
                                    workspace_id = workspaceId,
                                });
                                break;
                            case "webhook_failed":
                                notificationService.SendToAdminsGroup(ev, workspaceId?.ToString()??"", new
                                {
                                    log_id = data.LngN("log_id"),
                                    config_name = data.Str("config_name"),
                                    event_type = data.Str("event_type"),
                                    failure_reason = data.Str("failure_reason"),
                                    retry_count = data.Int("retry_count"),
                                    triggered_at = data.Str("triggered_at"),
                                    workspace_id = workspaceId,
                                });
                                break;
                            case "webhook_queued":
                                notificationService.SendToAdminsGroup(ev, workspaceId?.ToString()??"", new
                                {
                                    log_id = data.LngN("log_id"),
                                    config_name = data.Str("config_name"),
                                    event_type = data.Str("event_type"),
                                    workspace_id = workspaceId,
                                });
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

        public async Task<WebhooksDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default)
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
                Webhooks: result.Webhooks== null ? null : new (
                    ActiveConfigs: result.Webhooks.ActiveConfigs,
                    Failed24h: result.Webhooks.Delivered24h,
                    Delivered24h: result.Webhooks.Delivered24h,
                    PendingNow: result.Webhooks.PendingNow,
                    HealthPct: result.Webhooks.HealthPct,
                    RecentFailures: result.Webhooks.RecentFailures== null ? [] :
                    result.Webhooks.RecentFailures.Select(recent => new WebhookFailureResponse(
                        LogId: recent.LogId,
                        ConfigName: recent.ConfigName,
                        EventType: recent.EventType,
                        FailureReason: recent.FailureReason,
                        RetryCount: recent.RetryCount,
                        TriggeredAt: recent.TriggeredAt
                        )).ToArray())
                );
        }
    }
}

using ACS.Models.Response.V1.WebhooksService.Dashboard;

namespace ACS.Webhook.WebService.Services.V1.Interfaces
{
    public interface IDashboardService
    {
        void StartNotify();

        Task<WebhooksDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default);
    }
}

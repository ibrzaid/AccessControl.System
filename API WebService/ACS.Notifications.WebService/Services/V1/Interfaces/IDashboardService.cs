using ACS.Models.Response.V1.NotificationsService.Dashboard;

namespace ACS.Notifications.WebService.Services.V1.Interfaces
{
    public interface IDashboardService
    {
        void StartNotify();
        Task<NotificationsDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default);
    }
}

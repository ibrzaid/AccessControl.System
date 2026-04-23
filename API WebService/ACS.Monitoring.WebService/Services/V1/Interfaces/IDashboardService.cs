using ACS.Models.Response.V1.MonitoringService.Dashboard;

namespace ACS.Monitoring.WebService.Services.V1.Interfaces
{
    public interface IDashboardService
    {
        void StartNotify();
        Task<MonitoringDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default);
    }
}

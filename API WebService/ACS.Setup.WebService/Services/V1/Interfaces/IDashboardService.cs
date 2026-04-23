using ACS.Models.Response.V1.SetupService.Dashboard;

namespace ACS.Setup.WebService.Services.V1.Interfaces
{
    public interface IDashboardService
    {
        void StartNotify();

        Task<SetupDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default);
    }
}

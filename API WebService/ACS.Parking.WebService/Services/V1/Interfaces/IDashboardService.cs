using ACS.Models.Response.V1.ParkingService.Dashboard;

namespace ACS.Parking.WebService.Services.V1.Interfaces
{
    public interface IDashboardService
    {
        void StartNotify();
        Task<ParkingDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default);
    }
}

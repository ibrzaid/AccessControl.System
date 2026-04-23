using ACS.Models.Response.V1.ANPRService.Dashboard;

namespace ACS.ANPR.WebService.Services.V1.Interfaces
{
    public interface IDashboardService
    {
        void StartNotify();



        Task<AnprDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the full dashboard snapshot.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <param name="requestId"></param>
        /// <param name="viewType"></param>
        /// <param name="days"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AnprDashboardProjectResponse> GetDashboardByProjectAsync(string workspaceId, int projectId, string userId, string requestId, string viewType = "daily", int days = 30, CancellationToken cancellationToken = default);
    }
}

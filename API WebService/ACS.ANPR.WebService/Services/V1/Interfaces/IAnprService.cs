using ACS.Models.Request.V1.ANPRService;
using ACS.Models.Response.V1.ANPRService.Anpr;

namespace ACS.ANPR.WebService.Services.V1.Interfaces
{
    public interface IAnprService
    {
        Task<AnprInsertResponse> InsertAync(AnprInsertRequest request, string workspaceId, int projectId, int projectAreaId, int zoneId, int accessPointId, int hardwareId, string userIp, string userAgent,
            string requestId, CancellationToken cancellationToken = default);
    }
}

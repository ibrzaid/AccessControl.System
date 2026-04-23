using ACS.Models.Request.V1.SetupService.Project;
using ACS.Models.Response.V1.SetupService.Project;

namespace ACS.Setup.WebService.Services.V1.Interfaces
{
    public interface IProjectService
    {
        Task<GetProjectsResponse> GetProjectsAsync(int workspaceId, int callerId, int limit, int offset, CancellationToken ct = default);
        Task<OperationProjectResultResponse> CreateProjectAsync(int workspaceId, int callerId, CreateProjectRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationProjectResultResponse> UpdateProjectAsync(int workspaceId, int callerId, int projectId, UpdateProjectRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default);
        Task<OperationProjectResultResponse> DeleteProjectAsync(int workspaceId, int callerId, int projectId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default);
    }
}

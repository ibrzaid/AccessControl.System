using ACS.Models.Response.V1.SetupService.HierarchyAccess;

namespace ACS.Setup.WebService.Services.V1.Interfaces
{
    public interface IHierarchyAccessService
    {
        /// <summary>
        /// Returns the hierarchy tree for the calling user starting at
        /// their scope level. result_level in the response tells the
        /// caller what the top-level data items are.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="callerWorkspaceId"></param>
        /// <param name="projectId"></param>
        /// <param name="projectAreaId"></param>
        /// <param name="zoneId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<HierarchyAccessResponse> GetHierarchyAccessAsync(
            int callerUserId,
            int callerWorkspaceId,
            int? projectId = null,
            int? projectAreaId = null,
            int? zoneId = null,
            CancellationToken cancellationToken = default);
    }
}

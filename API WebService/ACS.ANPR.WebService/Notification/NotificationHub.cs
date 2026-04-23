using ACS.Helper.V1;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using ACS.ANPR.WebService.Services.V1.Interfaces;

namespace ACS.ANPR.WebService.Notification
{
    [Authorize]
    public class NotificationHub(FindClaimHelper claimHelper, INotificationService notificationService, ILogger<NotificationHub> logger) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            try
            {
                string userId = claimHelper.FindClaim(Context, ClaimTypes.NameIdentifier);
                string workspaceId = claimHelper.FindClaim(Context, "wid");
                string scope = claimHelper.FindClaim(Context, "lvl");
                if (userId == null || workspaceId == null)
                {
                    logger.LogWarning("[Hub] Rejected — missing user_id or workspace_id in JWT");
                    Context.Abort();
                    return;
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, notificationService.DashboardGroup(workspaceId));
                await Groups.AddToGroupAsync(Context.ConnectionId, notificationService.UserGroup(workspaceId, userId));
                if(scope is "ADMIN" or "SUPER_ADMIN" or "WORKSPACE_ADMIN")
                    await Groups.AddToGroupAsync(Context.ConnectionId, notificationService.AdminsGroup(workspaceId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing notifications.");
            }

            await base.OnConnectedAsync();
        }


        /// <summary>
        /// Joins project_{project_id} group to receive project-scoped events
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task WatchProject(string projectId)
        {
            string userId = claimHelper.FindClaim(Context, ClaimTypes.NameIdentifier);
            string workspace = claimHelper.FindClaim(Context, "wid");
            if (Context.Items["watching_project"] is string prev)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, prev);
                Context.Items.Remove("watching_project");
            }
            var groupName = notificationService.ProjectGroup(workspace, projectId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Context.Items["watching_project"] = groupName;
            logger.LogDebug("[Hub] user={User} → watching project={ProjectId}", userId, projectId);
        }

        /// <summary>
        /// Client calls this when navigating AWAY from a project pag
        /// </summary>
        /// <returns></returns>
        public async Task UnwatchProject()
        {
            if (Context.Items["watching_project"] is string prev)
            {
                string userId = claimHelper.FindClaim(Context, ClaimTypes.NameIdentifier);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, prev);
                Context.Items.Remove("watching_project");
                logger.LogDebug("[Hub] user={User} ← left project group", userId);
            }
        }

        /// <summary>
        /// This method is called when a user disconnects from the hub.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userId = claimHelper.FindClaim(Context, ClaimTypes.NameIdentifier);
            string workspaceId = claimHelper.FindClaim(Context, "wid");
            string scope = claimHelper.FindClaim(Context, "lvl");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, notificationService.DashboardGroup(workspaceId));
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, notificationService.UserGroup(workspaceId, userId));
            if (scope is "ADMIN" or "SUPER_ADMIN" or "WORKSPACE_ADMIN")
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, notificationService.AdminsGroup(workspaceId));

            await base.OnDisconnectedAsync(exception);
        }
    }
}

using ACS.Background;
using System.Text.Json;
using ACS.Service.V1.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using ACS.Webhook.WebService.Notification;
using ACS.Webhook.WebService.Services.V1.Interfaces;

namespace ACS.Webhook.WebService.Services.V1.Services
{
    public class NotificationService(ILicenseManager licenseManager, IBackgroundTaskQueue backgroundTaskQueue, IHubContext<NotificationHub> hubContext) : Service.Service(licenseManager), INotificationService
    {
        public string DashboardGroup(string workspace) => $"dashboard_{workspace}";
        public string UserGroup(string workspace, string user) => $"user_{workspace}:{user}";
        public string AdminsGroup(string workspace) => $"admins_{workspace}";
        public string ProjectGroup(string workspace, string project) => $"project_{workspace}:{project}";


        public void SendToWorkspaceGroup(string ev, string workspace, object payload) =>
             backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
             {
                 await hubContext.Clients.Group(DashboardGroup(workspace)).SendAsync(ev, payload, cancellationToken: token);
             });

        public void SendToProjectGroup(string ev, string workspace, string project, object payload) =>
             backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
             {
                 await hubContext.Clients.Group(ProjectGroup(workspace, project)).SendAsync(ev, payload, cancellationToken: token);
             });

        public void SendToAdminsGroup(string ev, string workspace, object payload) =>
             backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
             {
                 await hubContext.Clients.Group(AdminsGroup(workspace)).SendAsync(ev, payload, cancellationToken: token);
             });

        public void SendToUserGroup(string ev, string workspace, string user, object payload) =>
             backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
             {
                 await hubContext.Clients.Group(UserGroup(workspace, user)).SendAsync(ev, payload, cancellationToken: token);
             });
    }
}

using ACS.Background;
using ACS.Service.V1.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using ACS.Parking.WebService.Notification;
using ACS.Parking.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.ParkingService.Entry;

namespace ACS.Parking.WebService.Services.V1.Services
{
    public class NotificationService(ILicenseManager licenseManager, IBackgroundTaskQueue backgroundTaskQueue,  IHubContext<NotificationHub> hubContext) : Service.Service(licenseManager), INotificationService
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _allClients
            = new(StringComparer.OrdinalIgnoreCase);

        public ConcurrentDictionary<string, HashSet<string>> AllClients => _allClients;

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
        public void AddClient(string clientId, string connectionId)
        {
            var connections = AllClients.GetOrAdd(clientId, _ => []);

            lock (connections)
            {
                connections.Add(connectionId);
            }
        }

        public void RemoveClient(string clientId, string connectionId)
        {
            if (AllClients.TryGetValue(clientId, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                        AllClients.TryRemove(clientId, out _);
                }
            }
        }

        public void SendParkingSession(string workspace, string project, ParkingSessionDataResponse parkingSession)
        {
            backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                await hubContext.Clients.All.SendAsync("RECEIVE_PARKING_SESSION", workspace, project, parkingSession, token);
            });
        }

        public void SendParkingSession(string workspace, string project, string projectArea, string zone, string accessPoint, ParkingSessionDataResponse parkingSession)
        {
            backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                await hubContext.Clients.All.SendAsync("RECEIVE_PARKING_SESSION", workspace, project, projectArea, zone, accessPoint, parkingSession, token);
            });
        }
    }
}

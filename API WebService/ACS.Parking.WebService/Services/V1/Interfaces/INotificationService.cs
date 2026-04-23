using System.Collections.Concurrent;
using ACS.Models.Response.V1.ParkingService.Entry;

namespace ACS.Parking.WebService.Services.V1.Interfaces
{
    public interface INotificationService
    {

        string DashboardGroup(string workspace);
        string UserGroup(string workspace, string user);
        string AdminsGroup(string workspace);
        string ProjectGroup(string workspace, string project);


        void SendToWorkspaceGroup(string ev, string workspace, object payload);
        void SendToProjectGroup(string ev, string workspace, string project, object payload);
        void SendToAdminsGroup(string ev, string workspace, object payload);
        void SendToUserGroup(string ev, string workspace, string user, object payload);

        ConcurrentDictionary<string, HashSet<string>> AllClients { get; }

        void AddClient(string clientId, string connectionId);
        void RemoveClient(string clientId, string connectionId);
        void SendParkingSession(string workspace, string project, ParkingSessionDataResponse parkingSession);
        void SendParkingSession(string workspace, string project, string projectArea, string zone, string accessPoint, ParkingSessionDataResponse parkingSession);
    }
}

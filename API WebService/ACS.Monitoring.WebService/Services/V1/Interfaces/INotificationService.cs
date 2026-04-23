

namespace ACS.Monitoring.WebService.Services.V1.Interfaces
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
    }
}

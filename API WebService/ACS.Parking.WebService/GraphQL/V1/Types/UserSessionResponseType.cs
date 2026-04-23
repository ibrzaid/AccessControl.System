using GraphQL.Types;
using ACS.Models.Response.V1.AuthenticationService.Account;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class UserSessionResponseType : ObjectGraphType<UserSessionResponse>
    {
        public UserSessionResponseType()
        {
            Name = "UserSession";
            Description = "User session information";

            Field(x => x.SessionId).Description("Session ID");
            Field(x => x.UserId, nullable: true).Description("User ID");
            Field(x => x.WorkspaceId, nullable: true).Description("Workspace ID");
            Field(x => x.TokenExpiresAt, nullable: true).Description("Token expires at");
            Field(x => x.RefreshTokenExpiresAt, nullable: true).Description("Refresh token expires at");
            Field(x => x.IpAddress, nullable: true).Description("IP address");
            Field(x => x.UserAgent, nullable: true).Description("User agent");
            Field(x => x.DeviceInfo, nullable: true).Description("Device info");
            Field(x => x.ClientId, nullable: true).Description("Client ID");
            Field(x => x.ClientVersion, nullable: true).Description("Client version");
            Field(x => x.IsActive).Description("Is active");
            Field(x => x.CreatedAt).Description("Created at");
            Field(x => x.LastAccessedAt, nullable: true).Description("Last accessed at");
            Field(x => x.LogoutAt, nullable: true).Description("Logout at");
            Field(x => x.Latitude, nullable: true).Description("Latitude");
            Field(x => x.Longitude, nullable: true).Description("Longitude");
            Field(x => x.FailedRefreshAttempts, nullable: true).Description("Failed refresh attempts");
        }
    }
}

using GraphQL.Types;
using ACS.Models.Response.V1.AuthenticationService.Account;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class UserProfileResponseType : ObjectGraphType<UserProfileResponse>
    {
        public UserProfileResponseType()
        {
            Name = "UserProfile";
            Description = "User profile information";

            Field(x => x.UserId).Description("User ID");
            Field(x => x.Username, nullable: true).Description("Username");
            Field(x => x.Email, nullable: true).Description("Email");
            Field(x => x.FullName, nullable: true).Description("Full name");
            Field(x => x.AvatarUrl, nullable: true).Description("Avatar URL");
            Field(x => x.Department, nullable: true).Description("Department");
            Field(x => x.JobTitle, nullable: true).Description("Job title");
            Field(x => x.Timezone, nullable: true).Description("Timezone");
            Field(x => x.Language, nullable: true).Description("Language");
            Field(x => x.LastLogin, nullable: true).Description("Last login");
            Field(x => x.CreatedAt).Description("Created at");
            Field(x => x.UpdatedAt).Description("Updated at");
            Field(x => x.WorkspaceId).Description("Workspace ID");
            Field(x => x.WorkspaceName, nullable: true).Description("Workspace name");
        }
    }
}

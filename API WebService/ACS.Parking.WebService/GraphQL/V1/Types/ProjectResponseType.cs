using ACS.Models.Response.V1.SetupService;
using GraphQL.Types;
using System.Text.Json;
using System.Xml.Linq;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class ProjectResponseType : ObjectGraphType<ProjectResponse>
    {
        public ProjectResponseType()
        {
            Name = "Project";
            Description = "Project information";

            Field(x => x.ProjectId).Description("Project ID");
            Field<StringGraphType>("projectNames")
                .Resolve(context => JsonSerializer.Serialize(context.Source.ProjectNames)
            ).Description("Project names");
            Field(x => x.ProjectDescription, nullable: true).Description("Project description");
            Field(x => x.ProjectAddress, nullable: true).Description("Project address");
            Field(x => x.ProjectCity, nullable: true).Description("Project city");
            Field(x => x.ProjectState, nullable: true).Description("Project state");
            Field(x => x.CountryId, nullable: true).Description("Country ID");
            Field(x => x.PostalCode, nullable: true).Description("Postal code");
            Field(x => x.ProjectLatitude, nullable: true).Description("Project latitude");
            Field(x => x.ProjectLongitude, nullable: true).Description("Project longitude");
            Field(x => x.Timezone, nullable: true).Description("Timezone");
            Field(x => x.ProjectIsPublic, nullable: true).Description("Is public");
            Field(x => x.CreatedAt).Description("Created at");
            Field(x => x.UpdatedAt).Description("Updated at");
        }
    }
}

using GraphQL.Types;
using ACS.Models.Response.V1.SetupService;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class WorkspaceResponseType : ObjectGraphType<WorkspaceResponse>
    {
        public WorkspaceResponseType()
        {
            Name = "Workspace";
            Description = "Workspace information";

            Field(x => x.WorkspaceId).Description("Workspace ID");
            Field(x => x.WorkspaceCode, nullable: true).Description("Workspace code");
            Field(x => x.WorkspaceName, nullable: true).Description("Workspace name");
            Field(x => x.Description, nullable: true).Description("Description");
            Field(x => x.LicenseStatus, nullable: true).Description("License status");
            Field(x => x.MaxHardware, nullable: true).Description("Max hardware");
            Field(x => x.MaxUsers, nullable: true).Description("Max users");
            Field(x => x.MaxParkingSpots, nullable: true).Description("Max parking spots");
            Field(x => x.MaxVehicleRecords, nullable: true).Description("Max vehicle records");
            Field(x => x.CurrentHardware, nullable: true).Description("Current hardware");
            Field(x => x.CurrentUsers, nullable: true).Description("Current users");
            Field(x => x.CurrentParkingSpots, nullable: true).Description("Current parking spots");
            Field(x => x.CurrentVehicleRecords, nullable: true).Description("Current vehicle records");
            Field(x => x.Timezone, nullable: true).Description("Timezone");
            Field(x => x.Language, nullable: true).Description("Language");
            Field(x => x.IsActive).Description("Is active");
            Field(x => x.ContractStartDate, nullable: true).Description("Contract start date");
            Field(x => x.ContractEndDate, nullable: true).Description("Contract end date");
            Field(x => x.CreatedAt).Description("Created at");
            Field(x => x.UpdatedAt).Description("Updated at");
        }
    }
}

using GraphQL.Types;
using System.Text.Json;
using ACS.Models.Response.V1.SetupService;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class AreaZoneResponseType : ObjectGraphType<AreaZoneResponse>
    {
        public AreaZoneResponseType()
        {
            Name = "AreaZone";
            Description = "Area zone information";

            Field(x => x.ZoneId).Description("Zone ID");
            Field(x => x.ZoneCode, nullable: true).Description("Zone code");
            Field<StringGraphType>("zoneNames")
                .Resolve(context => JsonSerializer.Serialize(context.Source.ZoneNames)
            ).Description("Zone names");
            Field(x => x.ProjectId, nullable: true).Description("Project ID");
            Field(x => x.ProjectAreaId, nullable: true).Description("Project area ID");
            Field(x => x.ParentZoneId, nullable: true).Description("Parent zone ID");
            Field(x => x.ZoneTypeId, nullable: true).Description("Zone type ID");
            Field(x => x.FloorNumber, nullable: true).Description("Floor number");
            Field(x => x.TotalSpots, nullable: true).Description("Total spots");
            Field(x => x.AvailableSpots, nullable: true).Description("Available spots");
            Field(x => x.CenterLatitude, nullable: true).Description("Center latitude");
            Field(x => x.CenterLongitude, nullable: true).Description("Center longitude");
            Field(x => x.AccessLevelId, nullable: true).Description("Access level ID");
            Field(x => x.IsActive).Description("Is active");
            Field(x => x.CreatedAt).Description("Created at");
            Field(x => x.UpdatedAt).Description("Updated at");
        }
    }
}

using GraphQL.Types;
using System.Text.Json;
using ACS.Models.Response.V1.SetupService;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class AccessPointResponseType : ObjectGraphType<AccessPointResponse>
    {
        public AccessPointResponseType()
        {
            Name = "AccessPoint";
            Description = "Access point information";

            Field(x => x.AccessPointId).Description("Access point ID");
            Field<StringGraphType>("accessPointName")
                .Resolve(  context => JsonSerializer.Serialize(context.Source.AccessPointName)
            ).Description("Access point names");
            
            Field(x => x.AccessPointTypeCode, nullable: true).Description("Access point type code");
            Field<StringGraphType>("accessPointTypeNames")
               .Resolve(context => JsonSerializer.Serialize(context.Source.AccessPointTypeNames)
            ).Description("Access point type names");
            Field(x => x.PositionX, nullable: true).Description("Position X");
            Field(x => x.PositionY, nullable: true).Description("Position Y");
            Field(x => x.OrientationDegrees, nullable: true).Description("Orientation degrees");
            Field(x => x.AccessPointLatitude, nullable: true).Description("Latitude");
            Field(x => x.AccessPointLongitude, nullable: true).Description("Longitude");
            Field(x => x.ZoneId, nullable: true).Description("Zone ID");
            Field(x => x.IsActive).Description("Is active");
            Field(x => x.CreatedAt).Description("Created at");
            Field(x => x.UpdatedAt).Description("Updated at");
        }
    }
}

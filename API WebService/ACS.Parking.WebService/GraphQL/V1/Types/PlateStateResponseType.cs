using GraphQL.Types;
using System.Text.Json;
using ACS.Models.Response.V1.MasterService.PlateState;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class PlateStateResponseType : ObjectGraphType<PlateStateResponse>
    {
        public PlateStateResponseType()
        {
            Name = "PlateState";
            Description = "Plate state information";

            Field(x => x.PlateStateId).Description("Plate state ID");
            Field<StringGraphType>("plateStateName")
                .Resolve(context => JsonSerializer.Serialize(context.Source.PlateStateName)
            ).Description("Plate state name");
            Field(x => x.CountryId, nullable: true).Description("Country ID");
            Field(x => x.IsActive).Description("Is active");
            Field(x => x.CreatedDate).Description("Created date");
            Field(x => x.UpdatedDate).Description("Updated date");
        }
    }
}

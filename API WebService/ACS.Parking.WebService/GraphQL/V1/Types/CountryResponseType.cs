using GraphQL.Types;
using System.Text.Json;
using ACS.Models.Response.V1.MasterService.County;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class CountryResponseType : ObjectGraphType<CountryResponse>
    {
        public CountryResponseType()
        {
            Name = "Country";
            Description = "Country information";

            Field(x => x.CountryId).Description("Country ID");
            Field(x => x.CountryCode).Description("Country code");
            Field<StringGraphType>( "countryNames")
                .Resolve(context => JsonSerializer.Serialize(context.Source.CountryNames)
            ).Description("Country names");
            Field(x => x.IsActive).Description("Is active");
            Field(x => x.CreatedDate).Description("Created date");
            Field(x => x.UpdatedDate).Description("Updated date");
            Field<StringGraphType>("alphabets")
                .Resolve(context => JsonSerializer.Serialize(context.Source.Alphabets)
            ).Description("Alphabets");
            Field(x => x.Digits).Description("Digits");
            Field(x => x.PatternRegex).Description("Pattern regex");
            Field(x => x.PatternDescription).Description("Pattern description");
            Field(x => x.PlateConfigCreatedAt, nullable: true).Description("Plate config created at");
            Field(x => x.PlateConfigUpdatedAt, nullable: true).Description("Plate config updated at");
        }
    }
}

using GraphQL.Types;
using System.Text.Json;
using ACS.Models.Response.V1.MasterService.PlateCategory;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class PlateCategoryResponseType : ObjectGraphType<PlateCategoryResponse>
    {
        public PlateCategoryResponseType()
        {
            Name = "PlateCategory";
            Description = "Plate category information";

            Field(x => x.PlateCategoryId).Description("Plate category ID");
            Field(x => x.CategoryCode).Description("Category code");
            Field<StringGraphType>("categoryNames")
                .Resolve(context => JsonSerializer.Serialize(context.Source.CategoryNames)
            ).Description("Category names");
            Field<StringGraphType>("categoryDescriptions")
                .Resolve(context => JsonSerializer.Serialize(context.Source.CategoryDescriptions)
            ).Description("Category descriptions");
            Field(x => x.IsActive).Description("Is active");
            Field(x => x.CreatedDate).Description("Created date");
            Field(x => x.UpdatedDate).Description("Updated date");
        }
    }
}

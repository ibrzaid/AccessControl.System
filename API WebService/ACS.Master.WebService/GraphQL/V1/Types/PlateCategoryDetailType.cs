using GraphQL.Types;
using ACS.Master.WebService.GraphQL.V1.DataLoader;
using ACS.Models.Response.V1.MasterService.PlateCategoryDetail;

namespace ACS.Master.WebService.GraphQL.V1.Types
{
    public class PlateCategoryDetailType : ObjectGraphType<PlateCategoryDetailRespones>
    {
        public PlateCategoryDetailType()
        {
            Name = "PlateCategoryDetail";
            Description = "Represents relationship between plate state, country, and category";

            Field(x => x.PlateStateId, type: typeof(IntGraphType))
                .Description("The plate state ID");

            Field(x => x.CountryId, type: typeof(IntGraphType))
                .Description("The country ID");

            Field(x => x.PlateCategoryId, type: typeof(IntGraphType))
                .Description("The plate category ID");

            Field(x => x.IsActive, type: typeof(BooleanGraphType))
                .Description("Whether this relationship is active");

            Field(x => x.CreatedDate, type: typeof(DateTimeGraphType))
                .Description("Creation date");

            Field(x => x.UpdatedDate, type: typeof(DateTimeGraphType))
                .Description("Last update date");


            Field<PlateStateType>("plateState")
                .Description("The plate state")
                .ResolveAsync(async context =>
                {
                    var plateStateId = context.Source.PlateStateId;
                    var dataLoader = context.RequestServices!.GetRequiredService<PlateStateDataLoader>();
                    return await dataLoader.LoadSingleAsync(plateStateId, context.CancellationToken);
                });

            Field<CountryType>("country")
                .Description("The country")
                .ResolveAsync(async context =>
                {
                    var countryId = context.Source.CountryId;
                    var dataLoader = context.RequestServices!.GetRequiredService<CountryDataLoader>();
                    return await dataLoader.LoadSingleAsync(countryId, context.CancellationToken);
                });

            Field<PlateCategoryType>("plateCategory")
                .Description("The plate category")
                .ResolveAsync(async context =>
                {
                    var plateCategoryId = context.Source.PlateCategoryId;
                    var dataLoader = context.RequestServices!.GetRequiredService<PlateCategoryDataLoader>();
                    return await dataLoader.LoadSingleAsync(plateCategoryId, context.CancellationToken);
                });
        }
    }
}
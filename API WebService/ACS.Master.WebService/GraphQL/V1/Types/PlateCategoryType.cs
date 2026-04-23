using GraphQL;
using GraphQL.Types;
using System.Text.Json;
using ACS.Master.WebService.GraphQL.V1.DataLoader;
using ACS.Models.Response.V1.MasterService.PlateCategory;

namespace ACS.Master.WebService.GraphQL.V1.Types
{
    public class PlateCategoryType : ObjectGraphType<PlateCategoryResponse>
    {
        public PlateCategoryType()
        {
            Name = "PlateCategory";
            Description = "Represents a plate category in the system";

            Field(x => x.PlateCategoryId, type: typeof(IntGraphType))
                .Description("The unique identifier of the plate category");

            Field(x => x.CategoryCode, type: typeof(StringGraphType))
                .Description("The category code");

            Field<StringGraphType>("categoryNames")
                .Description("Multilingual category names as JSON")
                .Resolve(context =>
                {
                    var dict = context.Source.CategoryNames ?? [];
                    return JsonSerializer.Serialize(dict);
                });

            Field<StringGraphType>("categoryDescriptions")
                .Description("Multilingual category descriptions as JSON")
                .Resolve(context =>
                {
                    var dict = context.Source.CategoryDescriptions ?? [];
                    return JsonSerializer.Serialize(dict);
                });

            Field(x => x.IsActive, type: typeof(BooleanGraphType))
                .Description("Whether the plate category is active");

            Field(x => x.CreatedDate, type: typeof(DateTimeGraphType))
                .Description("Creation date");

            Field(x => x.UpdatedDate, type: typeof(DateTimeGraphType))
                .Description("Last update date");

           
      

            Field<ListGraphType<PlateCategoryDetailType>>("plateCategoryDetails")
            .Description("Plate category details for this category")
            .Argument<BooleanGraphType>("isActive", "Filter by active status")
            .Argument<IntGraphType>("plateStateId", "Filter by plate state ID")
            .Argument<IntGraphType>("countryId", "Filter by country ID")
            .ResolveAsync(async context =>
            {
                var plateCategoryId = context.Source.PlateCategoryId;
                var isActive = context.GetArgument<bool?>("isActive");
                var plateStateId = context.GetArgument<int?>("plateStateId");
                var countryId = context.GetArgument<int?>("countryId");

                var dataLoader = context.RequestServices!.GetRequiredService<PlateCategoryDetailsDataLoader>();
                var allDetails = await dataLoader.LoadSingleAsync(plateCategoryId, context.CancellationToken) ?? [];

                var filtered = allDetails.AsEnumerable();

                if (isActive.HasValue)
                    filtered = filtered.Where(d => d.IsActive == isActive.Value);

                if (plateStateId.HasValue)
                    filtered = filtered.Where(d => d.PlateStateId == plateStateId.Value);

                if (countryId.HasValue)
                    filtered = filtered.Where(d => d.CountryId == countryId.Value);

                return filtered.ToList();
            });
        }
    }
}
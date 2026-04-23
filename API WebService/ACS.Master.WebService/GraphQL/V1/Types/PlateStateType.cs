using GraphQL;
using GraphQL.Types;
using System.Text.Json;
using ACS.Master.WebService.GraphQL.V1.DataLoader;
using ACS.Models.Response.V1.MasterService.PlateState;

namespace ACS.Master.WebService.GraphQL.V1.Types
{
    public class PlateStateType : ObjectGraphType<PlateStateResponse>
    {
        public PlateStateType()
        {
            Name = "PlateState";
            Description = "Represents a plate state/province";

            Field(x => x.PlateStateId)
                .Description("Unique identifier for the plate state");

            Field<StringGraphType>("plateStateName")
                .Description("Multilingual plate state name as JSON")
                .Resolve(context =>
                {
                    var dict = context.Source.PlateStateName ?? [];
                    return JsonSerializer.Serialize(dict);
                });

            Field(x => x.CountryId, nullable: true)
                .Description("Country ID this plate state belongs to");

            Field(x => x.IsActive)
                .Description("Whether the plate state is active");

            Field(x => x.CreatedDate)
                .Description("Creation date");

            Field(x => x.UpdatedDate)
                .Description("Last update date");

          
            Field<CountryType>("country")
                .Description("The country this plate state belongs to")
                .ResolveAsync(async context =>
                {
                    if (!context.Source.CountryId.HasValue) return null;

                    var countryId = context.Source.CountryId.Value;
                    var dataLoader = context.RequestServices!.GetRequiredService<CountryDataLoader>();
                    return await dataLoader.LoadSingleAsync(countryId, context.CancellationToken);
                });

          
          

            Field<ListGraphType<PlateCategoryDetailType>>("plateCategoryDetails")
            .Description("Plate category details for this plate state")
            .Argument<BooleanGraphType>("isActive", "Filter by active status")
            .Argument<IntGraphType>("plateCategoryId", "Filter by plate category ID")
            .ResolveAsync(async context =>
            {
                var plateStateId = context.Source.PlateStateId;
                var isActive = context.GetArgument<bool?>("isActive");
                var plateCategoryId = context.GetArgument<int?>("plateCategoryId");

                var dataLoader = context.RequestServices!.GetRequiredService<PlateStateCategoryDetailsDataLoader>();
                var allDetails = await dataLoader.LoadSingleAsync(plateStateId, context.CancellationToken) ?? [];

                var filtered = allDetails.AsEnumerable();

                if (isActive.HasValue)
                    filtered = filtered.Where(d => d.IsActive == isActive.Value);

                if (plateCategoryId.HasValue)
                    filtered = filtered.Where(d => d.PlateCategoryId == plateCategoryId.Value);

                return filtered.ToList();
            });
        }
    }
}
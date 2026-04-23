using GraphQL;
using GraphQL.Types;
using System.Text.Json;
using ACS.Master.WebService.GraphQL.V1.DataLoader;
using ACS.Models.Response.V1.MasterService.County;

namespace ACS.Master.WebService.GraphQL.V1.Types
{
    public class CountryType : ObjectGraphType<CountryResponse>
    {
        public CountryType()
        {
            Name = "Country";
            Description = "Represents a country in the system";

            Field(x => x.CountryId, type: typeof(IntGraphType))
                .Description("The unique identifier of the country");

            Field(x => x.CountryCode, type: typeof(StringGraphType))
                .Description("The ISO country code");

            Field<StringGraphType>("countryNames")
                .Description("Multilingual country names as JSON")
                .Resolve(context =>
                {
                    var dict = context.Source.CountryNames ?? [];
                    return JsonSerializer.Serialize(dict);
                });

            Field(x => x.IsActive, type: typeof(BooleanGraphType))
                .Description("Whether the country is active");

            Field(x => x.CreatedDate, type: typeof(DateTimeGraphType))
                .Description("Creation date");

            Field(x => x.UpdatedDate, type: typeof(DateTimeGraphType))
                .Description("Last update date");


            Field<StringGraphType>("alphabets")
                .Description("Available alphabets for plate codes by language (JSON)")
                .Resolve(context =>
                {
                    var dict = context.Source.Alphabets ?? [];
                    return JsonSerializer.Serialize(dict);
                });

            Field(x => x.Digits, type: typeof(StringGraphType))
                .Description("Available digits for plate numbers")
                .DefaultValue("0123456789");

            Field(x => x.PatternRegex, type: typeof(StringGraphType))
                .Description("Regex pattern for plate validation");

            Field(x => x.PatternDescription, type: typeof(StringGraphType))
                .Description("Human-readable description of the plate pattern");

            Field(x => x.PlateConfigCreatedAt, type: typeof(DateTimeGraphType))
                .Description("Plate configuration creation date");

            Field(x => x.PlateConfigUpdatedAt, type: typeof(DateTimeGraphType))
                .Description("Plate configuration last update date");

            Field<ListGraphType<StringGraphType>>("alphabetsByLanguage")
                .Description("Get alphabets for a specific language")
                .Argument<StringGraphType>("languageCode", "Language code (EN, AR, etc.)")
                .Resolve(context =>
                {
                    var alphabets = context.Source.Alphabets ?? [];
                    var languageCode = context.GetArgument<string>("languageCode")?.ToUpper() ?? "EN";

                    if (alphabets.TryGetValue(languageCode, out var languageAlphabets))
                        return languageAlphabets;

                    return alphabets.TryGetValue("EN", out var englishAlphabets) ? englishAlphabets : [];
                });

            Field<BooleanGraphType>("hasPlateConfig")
                .Description("Whether this country has plate configuration")
                .Resolve(context =>
                    !string.IsNullOrEmpty(context.Source.PatternRegex) ||
                    (context.Source.Alphabets?.Count ?? 0) > 0);


            Field<ListGraphType<PlateStateType>>("plateStates")
                .Description("Plate states in this country")
                .Argument<BooleanGraphType>("isActive", "Filter by active status")
                .ResolveAsync(async context =>
                {
                    var countryId = context.Source.CountryId;
                    var isActive = context.GetArgument<bool?>("isActive");
                    var dataLoader = context.RequestServices!.GetRequiredService<CountryPlateStatesDataLoader>();
                    var allStates = await dataLoader.LoadSingleAsync(countryId, context.CancellationToken) ?? [];

                    if (isActive.HasValue)
                        return allStates.Where(s => s.IsActive == isActive.Value).ToList();

                    return allStates;
                });

            Field<ListGraphType<PlateCategoryDetailType>>("plateCategoryDetails")
                .Description("Plate category details for this country")
                .Argument<BooleanGraphType>("isActive", "Filter by active status")
                .Argument<IntGraphType>("plateCategoryId", "Filter by plate category ID")
                .ResolveAsync(async context =>
                {
                    var countryId = context.Source.CountryId;
                    var isActive = context.GetArgument<bool?>("isActive");
                    var plateCategoryId = context.GetArgument<int?>("plateCategoryId");
                    var dataLoader = context.RequestServices!.GetRequiredService<CountryPlateCategoryDetailsDataLoader>();
                    var allDetails = await dataLoader.LoadSingleAsync(countryId, context.CancellationToken) ?? [];

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

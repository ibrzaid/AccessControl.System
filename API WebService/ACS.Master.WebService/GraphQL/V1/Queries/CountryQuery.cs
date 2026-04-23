using GraphQL;
using GraphQL.Types;
using ACS.Service.V1.Interfaces;
using ACS.Master.WebService.GraphQL.V1.Types;
using ACS.Master.WebService.GraphQL.V1.DataLoader;
using ACS.Models.Response.V1.MasterService.County;

namespace ACS.Master.WebService.GraphQL.V1.Queries
{
    public class CountryQuery : ObjectGraphType
    {
        public CountryQuery(ICountryService countryService)
        {
            Name = "CountryQuery";

            Field<ListGraphType<CountryType>>("countries")
            .Description("Get all countries with filtering")
            .Arguments(
                new QueryArguments(
                    new QueryArgument<BooleanGraphType> { Name = "isActive", Description = "Filter by active status" },
                    new QueryArgument<StringGraphType> { Name = "countryCode", Description = "Filter by country code" },
                    new QueryArgument<StringGraphType> { Name = "search", Description = "Search in country code or names" },
                    new QueryArgument<IntGraphType> { Name = "page", DefaultValue = 1, Description = "Page number" },
                    new QueryArgument<IntGraphType> { Name = "pageSize", DefaultValue = 50, Description = "Page size" },
                    new QueryArgument<StringGraphType> { Name = "sortBy", Description = "Field to sort by" },
                    new QueryArgument<StringGraphType> { Name = "sortOrder", DefaultValue = "asc", Description = "Sort order" }
                ))
                .ResolveAsync(async context =>
                {
                    var isActive = context.GetArgument<bool?>("isActive");
                    var countryCode = context.GetArgument<string>("countryCode");
                    var search = context.GetArgument<string>("search");
                    var page = context.GetArgument<int>("page");
                    var pageSize = context.GetArgument<int>("pageSize");
                    var sortBy = context.GetArgument<string>("sortBy");
                    var sortOrder = context.GetArgument<string>("sortOrder");
                    List<CountryResponse> results;
                    if (!string.IsNullOrEmpty(countryCode))results = await countryService.GetCountriesByCodeAsync(countryCode, isActive, context.CancellationToken);
                    else if (!string.IsNullOrEmpty(search)) results = await countryService.SearchCountriesAsync(search, isActive, context.CancellationToken);
                    else results = await countryService.GetAllCountriesAsync(isActive, context.CancellationToken);
                    if (results == null || results.Count == 0) return new List<CountryResponse>();
                    if (!string.IsNullOrEmpty(sortBy)) results = SortCountries(results, sortBy, sortOrder);

                    return results
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                });

            Field<ListGraphType<CountryType>>("countriesByCode")
            .Description("Get countries by country code")
            .Argument<NonNullGraphType<StringGraphType>>("countryCode", "The country code")
            .Argument<BooleanGraphType>("isActive", "Filter by active status")
            .ResolveAsync(async context =>
            {
                var countryCode = context.GetArgument<string>("countryCode");
                var isActive = context.GetArgument<bool?>("isActive");
                return await countryService.GetCountriesByCodeAsync(countryCode, isActive, context.CancellationToken);
            });

            Field<ListGraphType<CountryType>>("searchCountries")
                .Description("Search countries by code or name")
                .Argument<NonNullGraphType<StringGraphType>>("searchTerm", "Search term")
                .Argument<BooleanGraphType>("isActive", "Filter by active status")
                .ResolveAsync(async context =>
                {
                    var searchTerm = context.GetArgument<string>("searchTerm");
                    var isActive = context.GetArgument<bool?>("isActive");
                    return await countryService.SearchCountriesAsync(searchTerm, isActive, context.CancellationToken);
                });

            Field<CountryType, CountryResponse?>("country")
                .Description("Get a country by ID")
                .Argument<NonNullGraphType<IntGraphType>>("id", "The country ID")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    var dataLoader = context.RequestServices!.GetRequiredService<CountryDataLoader>();
                    return await dataLoader.LoadSingleAsync(id, context.CancellationToken);
                });

            Field<ListGraphType<CountryType>>("countriesByIds")
                .Description("Get multiple countries by IDs (batched)")
                .Argument<NonNullGraphType<ListGraphType<IntGraphType>>>("ids", "List of country IDs")
                .ResolveAsync(async context =>
                {
                    var ids = context.GetArgument<List<int>>("ids");
                    var dataLoader = context.RequestServices!.GetRequiredService<CountryDataLoader>();

                    var tasks = ids.Select(id => dataLoader.LoadSingleAsync(id, context.CancellationToken));
                    var results = await Task.WhenAll(tasks);

                    return results.Where(r => r != null).ToList();
                });

            Field<ListGraphType<CountryType>>("activeCountries")
                .Description("Get all active countries")
                .ResolveAsync(async context =>
                {
                    return await countryService.GetAllCountriesAsync(true, context.CancellationToken)
                        ?? [];
                });
        }

        private static List<CountryResponse> SortCountries(List<CountryResponse> countries, string sortBy, string sortOrder)
        {
            return sortBy.ToLower() switch
            {
                "countryid" => sortOrder == "desc"
                    ? [.. countries.OrderByDescending(c => c.CountryId)]
                    : [.. countries.OrderBy(c => c.CountryId)],
                "countrycode" => sortOrder == "desc"
                    ? [.. countries.OrderByDescending(c => c.CountryCode)]
                    : [.. countries.OrderBy(c => c.CountryCode)],
                "createddate" => sortOrder == "desc"
                    ? [.. countries.OrderByDescending(c => c.CreatedDate)]
                    : [.. countries.OrderBy(c => c.CreatedDate)],
                _ => countries
            };
        }
    }
}
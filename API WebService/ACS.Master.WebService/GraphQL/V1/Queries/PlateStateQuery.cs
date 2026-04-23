using GraphQL;
using GraphQL.Types;
using ACS.Service.V1.Interfaces;
using ACS.Master.WebService.GraphQL.V1.Types;
using ACS.Master.WebService.GraphQL.V1.DataLoader;
using ACS.Models.Response.V1.MasterService.PlateState;

namespace ACS.Master.WebService.GraphQL.V1.Queries
{
    public class PlateStateQuery : ObjectGraphType
    {
        public PlateStateQuery(IPlateStateService plateStateService)
        {
            Name = "PlateStateQuery";

            Field<ListGraphType<PlateStateType>>("plateStates")
                .Description("Get plate states with filtering")
                .Arguments(
                    new QueryArguments(
                        new QueryArgument<BooleanGraphType> { Name = "isActive", Description = "Filter by active status" },
                        new QueryArgument<StringGraphType> { Name = "search", Description = "Search in plate state names" },
                        new QueryArgument<IntGraphType> { Name = "countryId", Description = "Filter by country ID" },
                        new QueryArgument<IntGraphType> { Name = "page", DefaultValue = 1, Description = "Page number" },
                        new QueryArgument<IntGraphType> { Name = "pageSize", DefaultValue = 50, Description = "Page size" },
                        new QueryArgument<StringGraphType> { Name = "sortBy", Description = "Field to sort by" },
                        new QueryArgument<StringGraphType> { Name = "sortOrder", DefaultValue = "asc", Description = "Sort order" }
                    ))
                .ResolveAsync(async context =>
                {
                    var isActive = context.GetArgument<bool?>("isActive");
                    var search = context.GetArgument<string>("search");
                    var countryId = context.GetArgument<int?>("countryId");
                    var page = context.GetArgument<int>("page");
                    var pageSize = context.GetArgument<int>("pageSize");
                    var sortOrder = context.GetArgument<string>("sortOrder");
                    var sortBy = context.GetArgument<string>("sortBy");


                    List<PlateStateResponse> results;
                    if (!string.IsNullOrEmpty(search) || countryId.HasValue)  results = await plateStateService.GetFilteredPlateStatesAsync(
                            isActive, search, countryId, context.CancellationToken);
                    else results = await plateStateService.GetAllPlateStatesAsync(isActive, context.CancellationToken);
                    if (results == null || results.Count == 0) return new List<PlateStateResponse>();
                    if (!string.IsNullOrEmpty(sortBy)) results = SortPlateStates(results, sortBy, sortOrder);

                    return results
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                });

            Field<ListGraphType<PlateStateType>>("searchPlateStates")
            .Description("Search plate states by name")
            .Argument<NonNullGraphType<StringGraphType>>("searchTerm", "Search term")
            .Argument<BooleanGraphType>("isActive", "Filter by active status")
            .ResolveAsync(async context =>
            {
                var searchTerm = context.GetArgument<string>("searchTerm");
                var isActive = context.GetArgument<bool?>("isActive");
                return await plateStateService.SearchPlateStatesAsync(searchTerm, isActive, context.CancellationToken);
            });


            Field<PlateStateType>("plateState")
                .Description("Get a plate state by ID")
                .Argument<NonNullGraphType<IntGraphType>>("id", "The plate state ID")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    var dataLoader = context.RequestServices!.GetRequiredService<PlateStateDataLoader>();
                    return await dataLoader.LoadSingleAsync(id, context.CancellationToken);
                });

            Field<ListGraphType<PlateStateType>>("plateStatesByCountry")
                .Description("Get plate states by country ID")
                .Argument<NonNullGraphType<IntGraphType>>("countryId", "The country ID")
                .Argument<BooleanGraphType>("isActive", "Filter by active status")
                .ResolveAsync(async context =>
                {
                    var countryId = context.GetArgument<int>("countryId");
                    var isActive = context.GetArgument<bool?>("isActive");
                    return await plateStateService.GetPlateStatesByCountryIdAsync(countryId, isActive, context.CancellationToken);
                });

            Field<ListGraphType<PlateStateType>>("plateStatesByIds")
                .Description("Get multiple plate states by IDs (batched)")
                .Argument<NonNullGraphType<ListGraphType<IntGraphType>>>("ids", "List of plate state IDs")
                .ResolveAsync(async context =>
                {
                    var ids = context.GetArgument<List<int>>("ids");
                    var dataLoader = context.RequestServices!.GetRequiredService<PlateStateDataLoader>();

                    var tasks = ids.Select(id => dataLoader.LoadSingleAsync(id, context.CancellationToken));
                    var results = await Task.WhenAll(tasks);

                    return results.Where(r => r != null).ToList();
                });
        }

        private static List<PlateStateResponse> SortPlateStates(List<PlateStateResponse> states, string sortBy, string sortOrder)
        {
            return sortBy.ToLower() switch
            {
                "platestateid" => sortOrder == "desc"
                    ? [.. states.OrderByDescending(s => s.PlateStateId)]
                    : [.. states.OrderBy(s => s.PlateStateId)],
                "createddate" => sortOrder == "desc"
                    ? [.. states.OrderByDescending(s => s.CreatedDate)]
                    : [.. states.OrderBy(s => s.CreatedDate)],
                "updateddate" => sortOrder == "desc"
                    ? [.. states.OrderByDescending(s => s.UpdatedDate)]
                    : [.. states.OrderBy(s => s.UpdatedDate)],
                _ => states
            };
        }
    }
}

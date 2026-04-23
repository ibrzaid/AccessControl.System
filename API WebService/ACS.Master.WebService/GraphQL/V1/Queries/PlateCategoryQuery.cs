using ACS.Master.WebService.GraphQL.V1.DataLoader;
using ACS.Master.WebService.GraphQL.V1.Types;
using ACS.Master.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.PlateCategory;
using GraphQL;
using GraphQL.Types;
using System.Xml.Linq;

namespace ACS.Master.WebService.GraphQL.V1.Queries
{
    public class PlateCategoryQuery : ObjectGraphType
    {
        public PlateCategoryQuery(IPlateCategoryService plateCategoryService)
        {
            Name = "PlateCategoryQuery";

            Field<ListGraphType<PlateCategoryType>>("plateCategories")
                .Description("Get plate categories with filtering")
                .Arguments(
                    new QueryArguments(
                        new QueryArgument<BooleanGraphType> { Name = "isActive", Description = "Filter by active status" },
                        new QueryArgument<StringGraphType> { Name = "categoryCode", Description = "Filter by category code" },
                        new QueryArgument<StringGraphType> { Name = "search", Description = "Search in category code, names or descriptions" },
                        new QueryArgument<IntGraphType> { Name = "page", DefaultValue = 1, Description = "Page number" },
                        new QueryArgument<IntGraphType> { Name = "pageSize", DefaultValue = 50, Description = "Page size" },
                        new QueryArgument<StringGraphType> { Name = "sortBy", Description = "Field to sort by" },
                        new QueryArgument<StringGraphType> { Name = "sortOrder", DefaultValue = "asc", Description = "Sort order" }
                    ))
                .ResolveAsync(async context =>
                {
                    var isActive = context.GetArgument<bool?>("isActive");
                    var categoryCode = context.GetArgument<string>("categoryCode");
                    var search = context.GetArgument<string>("search");
                    var page = context.GetArgument<int>("page");
                    var pageSize = context.GetArgument<int>("pageSize");
                    var sortBy = context.GetArgument<string>("sortBy");
                    var sortOrder = context.GetArgument<string>("sortOrder");

                    List<PlateCategoryResponse> results;
                    if (!string.IsNullOrEmpty(categoryCode)) results = await plateCategoryService.GetPlateCategoriesByCodeAsync(categoryCode, isActive, context.CancellationToken);
                    else if (!string.IsNullOrEmpty(search)) results = await plateCategoryService.SearchPlateCategoriesAsync(search, isActive, context.CancellationToken);
                    else results = await plateCategoryService.GetAllPlateCategoriesAsync(isActive, context.CancellationToken);
                    if (results == null || results.Count == 0)  return new List<PlateCategoryResponse>();
                    if (!string.IsNullOrEmpty(sortBy)) results = SortPlateCategories(results, sortBy, sortOrder);

                    return results
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                });

            Field<ListGraphType<PlateCategoryType>>("plateCategoriesByCode")
            .Description("Get plate categories by category code")
            .Argument<NonNullGraphType<StringGraphType>>("categoryCode", "The category code")
            .Argument<BooleanGraphType>("isActive", "Filter by active status")
            .ResolveAsync(async context =>
            {
                var categoryCode = context.GetArgument<string>("categoryCode");
                var isActive = context.GetArgument<bool?>("isActive");
                return await plateCategoryService.GetPlateCategoriesByCodeAsync(categoryCode, isActive, context.CancellationToken);
            });

            Field<ListGraphType<PlateCategoryType>>("searchPlateCategories")
                .Description("Search plate categories")
                .Argument<NonNullGraphType<StringGraphType>>("searchTerm", "Search term")
                .Argument<BooleanGraphType>("isActive", "Filter by active status")
                .ResolveAsync(async context =>
                {
                    var searchTerm = context.GetArgument<string>("searchTerm");
                    var isActive = context.GetArgument<bool?>("isActive");
                    return await plateCategoryService.SearchPlateCategoriesAsync(searchTerm, isActive, context.CancellationToken);
                });

            Field<PlateCategoryType>("plateCategory")
                .Description("Get a plate category by ID")
                .Argument<NonNullGraphType<IntGraphType>>("id", "The plate category ID")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    var dataLoader = context.RequestServices!.GetRequiredService<PlateCategoryDataLoader>();
                    return await dataLoader.LoadSingleAsync(id, context.CancellationToken);
                });

            Field<ListGraphType<PlateCategoryType>>("plateCategoriesByIds")
                .Description("Get multiple plate categories by IDs")
                .Argument<NonNullGraphType<ListGraphType<IntGraphType>>>("ids", "List of plate category IDs")
                .ResolveAsync(async context =>
                {
                    var ids = context.GetArgument<List<int>>("ids");
                    var dataLoader = context.RequestServices!.GetRequiredService<PlateCategoryDataLoader>();

                    // Load all using DataLoader (will be batched)
                    var tasks = ids.Select(id => dataLoader.LoadSingleAsync(id, context.CancellationToken));
                    var results = await Task.WhenAll(tasks);

                    return results.Where(r => r != null).ToList();
                });

            Field<ListGraphType<PlateCategoryType>>("activePlateCategories")
                 .Description("Get all active plate categories")
                 .ResolveAsync(async context =>
                 {
                     return await plateCategoryService.GetAllPlateCategoriesAsync(true, context.CancellationToken)
                         ?? [];
                 });
        }

        private static List<PlateCategoryResponse> SortPlateCategories(List<PlateCategoryResponse> categories, string sortBy, string sortOrder)
        {
            if (categories == null || categories.Count == 0) return [];

            return sortBy.ToLower() switch
            {
                "platecategoryid" => sortOrder == "desc"
                    ? [.. categories.OrderByDescending(c => c.PlateCategoryId)]
                    : [.. categories.OrderBy(c => c.PlateCategoryId)],
                "categorycode" => sortOrder == "desc"
                    ? [.. categories.OrderByDescending(c => c.CategoryCode)]
                    : [.. categories.OrderBy(c => c.CategoryCode)],
                "createddate" => sortOrder == "desc"
                    ? [.. categories.OrderByDescending(c => c.CreatedDate)]
                    : [.. categories.OrderBy(c => c.CreatedDate)],
                "updateddate" => sortOrder == "desc"
                    ? [.. categories.OrderByDescending(c => c.UpdatedDate)]
                    : [.. categories.OrderBy(c => c.UpdatedDate)],
                _ => categories
            };
        }
    }
}
using GraphQL;
using GraphQL.Types;
using ACS.Master.WebService.GraphQL.V1.Types;
using ACS.Master.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.PlateCategoryDetail;
using System.Xml.Linq;

namespace ACS.Master.WebService.GraphQL.V1.Queries
{
    public class PlateCategoryDetailQuery : ObjectGraphType
    {
        public PlateCategoryDetailQuery(IPlateCategoryDetailService plateCategoryDetailService)
        {
            Field<ListGraphType<PlateCategoryDetailType>>("plateCategoryDetails")
            .Description("Get plate category details with filtering")
            .Arguments(
                new QueryArguments(
                    new QueryArgument<BooleanGraphType> { Name = "isActive", Description = "Filter by active status" },
                    new QueryArgument<IntGraphType> { Name = "plateStateId", Description = "Filter by plate state ID" },
                    new QueryArgument<IntGraphType> { Name = "countryId", Description = "Filter by country ID" },
                    new QueryArgument<IntGraphType> { Name = "plateCategoryId", Description = "Filter by plate category ID" },
                    new QueryArgument<IntGraphType> { Name = "page", DefaultValue = 1, Description = "Page number" },
                    new QueryArgument<IntGraphType> { Name = "pageSize", DefaultValue = 50, Description = "Page size" }
                ))
            .ResolveAsync(async context =>
            {
                var isActive = context.GetArgument<bool?>("isActive");
                var plateStateId = context.GetArgument<int?>("plateStateId");
                var countryId = context.GetArgument<int?>("countryId");
                var plateCategoryId = context.GetArgument<int?>("plateCategoryId");
                var page = context.GetArgument<int>("page");
                var pageSize = context.GetArgument<int>("pageSize");
                var results = await plateCategoryDetailService.GetFilteredPlateCategoryDetailsAsync( isActive, plateStateId, countryId, plateCategoryId, context.CancellationToken);

                if (results == null || results.Count == 0) return new List<PlateCategoryDetailRespones>();

                return results
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            });

            Field<ListGraphType<PlateCategoryDetailType>>("activeDetailsByStateAndCategory")
            .Description("Get active plate category details for specific state and category")
            .Argument<NonNullGraphType<IntGraphType>>("plateStateId", "The plate state ID")
            .Argument<NonNullGraphType<IntGraphType>>("plateCategoryId", "The plate category ID")
            .ResolveAsync(async context =>
            {
                var plateStateId = context.GetArgument<int>("plateStateId");
                var plateCategoryId = context.GetArgument<int>("plateCategoryId");
                return await plateCategoryDetailService.GetActiveDetailsByStateAndCategoryAsync(
                    plateStateId, plateCategoryId, context.CancellationToken);
            });


            Field<PlateCategoryDetailType>("plateCategoryDetail")
                .Description("Get a plate category detail by keys")
                .Argument<NonNullGraphType<IntGraphType>>("plateStateId", "The plate state ID")
                .Argument<NonNullGraphType<IntGraphType>>("countryId", "The country ID")
                .Argument<NonNullGraphType<IntGraphType>>("plateCategoryId", "The plate category ID")
                .ResolveAsync(async context =>
                {
                    var plateStateId = context.GetArgument<int>("plateStateId");
                    var countryId = context.GetArgument<int>("countryId");
                    var plateCategoryId = context.GetArgument<int>("plateCategoryId");

                    return await plateCategoryDetailService.GetPlateCategoryDetailByKeysAsync(
                        plateStateId, countryId, plateCategoryId, context.CancellationToken);
                });

            Field<ListGraphType<PlateCategoryDetailType>>("detailsByPlateState")
                .Description("Get plate category details by plate state ID")
                .Argument<NonNullGraphType<IntGraphType>>("plateStateId", "The plate state ID")
                .Argument<BooleanGraphType>("isActive", "Filter by active status")
                .ResolveAsync(async context =>
                {
                    var plateStateId = context.GetArgument<int>("plateStateId");
                    var isActive = context.GetArgument<bool?>("isActive");
                    return await plateCategoryDetailService.GetDetailsByPlateStateIdAsync(plateStateId, isActive, context.CancellationToken);
                });

            Field<ListGraphType<PlateCategoryDetailType>>("detailsByCountry")
                .Description("Get plate category details by country ID")
                .Argument<NonNullGraphType<IntGraphType>>("countryId", "The country ID")
                .Argument<BooleanGraphType>("isActive", "Filter by active status")
                .ResolveAsync(async context =>
                {
                    var countryId = context.GetArgument<int>("countryId");
                    var isActive = context.GetArgument<bool?>("isActive");
                    return await plateCategoryDetailService.GetDetailsByCountryIdAsync(countryId, isActive, context.CancellationToken);
                });

            Field<ListGraphType<PlateCategoryDetailType>>("detailsByCategory")
                .Description("Get plate category details by category ID")
                .Argument<NonNullGraphType<IntGraphType>>("plateCategoryId", "The plate category ID")
                .Argument<BooleanGraphType>("isActive", "Filter by active status")
                .ResolveAsync(async context =>
                {
                    var plateCategoryId = context.GetArgument<int>("plateCategoryId");
                    var isActive = context.GetArgument<bool?>("isActive");
                    return await plateCategoryDetailService.GetDetailsByCategoryIdAsync(plateCategoryId, isActive, context.CancellationToken);
                });
        }
    }
}

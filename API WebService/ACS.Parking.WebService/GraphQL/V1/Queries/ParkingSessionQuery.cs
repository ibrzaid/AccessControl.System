using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Parking.WebService.GraphQL.V1.DataLoader;
using ACS.Parking.WebService.GraphQL.V1.Types;
using ACS.Parking.WebService.Services.V1.Interfaces;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;

namespace ACS.Parking.WebService.GraphQL.V1.Queries
{
    public class ParkingSessionQuery: ObjectGraphType
    {
        public ParkingSessionQuery(ISessionService sessionService, ProjectSessionsDataLoader projectSessionsDataLoader ) 
        {
            Name = "ParkingSessionQuery";
            Description = "Query for parking sessions";

           // Field<ParkingSessionType>("parkingSession")
           //     .Description("Get a parking session by ID")
           //     .Arguments(new QueryArguments(
           //        new QueryArgument<LongGraphType> { Name = "id",  Description = "Parking session ID", DefaultValue = 0  }
           //    ))
           //     .ResolveAsync(async context =>
           //     {
           //        var sessionId = context.GetArgument<long>("id");
           //        if (sessionId <= 0) return null;

           //        var loader = dataLoaderContextAccessor.Context
           //            ?.GetOrAddBatchLoader<long, ParkingSessionDataResponse?>(
           //                "ParkingSessionById",
           //                async (keys, ct) =>
           //                {
           //                    var workspace = "default";
           //                    var results = await sessionService.GetParkingSessionsByIdsBatchAsync(
           //                        keys.ToList(), workspace, ct);
           //                    return null;

           //                    //return results.ToDictionary(
           //                    //    kv => kv.Key,
           //                    //    kv => kv.Value?.Success == true ? kv.Value.Data : null);
           //                });

           //        return loader?.LoadAsync(sessionId);
           //    }
           //);

            // Get parking sessions by project
            Field<SessionsResponseType>("parkingSessionsByProject")
                .Description("Get parking sessions by project")
               .Arguments(new QueryArguments(
                   new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "workspaceId", Description = "Workspace ID" },
                   new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "projectId", Description = "Project ID" },
                   new QueryArgument<IntGraphType> { Name = "skip", Description = "Number of records to skip", DefaultValue = 0 },
                   new QueryArgument<IntGraphType> { Name = "take", Description = "Number of records to take", DefaultValue = 20 },
                   new QueryArgument<StringGraphType> { Name = "status", Description = "Filter by status" }
                ))
                 .ResolveAsync(async context =>
                 {
                     var projectId = context.GetArgument<int>("projectId");
                     var workspaceId = context.GetArgument<int>("workspaceId");
                     var skip = context.GetArgument<int>("skip");
                     var take = context.GetArgument<int>("take");
                     var status = context.GetArgument<string?>("status");
                     return await projectSessionsDataLoader.LoadSingleAsync( 
                         workspaceId,
                         projectId,
                         skip,
                         take,
                         status,
                         
                         context.CancellationToken);
                 }
            );

            // Get parking sessions by zone
            //Field<ListGraphType<ParkingSessionType>>("parkingSessionsByZone")
            //    .Description("Get parking sessions by zone")
            //    .Arguments(new QueryArguments(
            //        new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "zoneId", Description = "Zone ID" },
            //        new QueryArgument<IntGraphType> { Name = "skip", Description = "Number of records to skip", DefaultValue = 0 },
            //        new QueryArgument<IntGraphType> { Name = "take", Description = "Number of records to take", DefaultValue = 20 },
            //        new QueryArgument<StringGraphType> { Name = "status", Description = "Filter by status" }
            //    ))
            //    .ResolveAsync(async context =>
            //    {
            //        var zoneId = context.GetArgument<int>("zoneId");
            //        var skip = context.GetArgument<int>("skip");
            //        var take = context.GetArgument<int>("take");
            //        var status = context.GetArgument<string?>("status");
            //        var workspace = "default";

            //        var response = await sessionService.GetParkingSessionsByZoneAsync(
            //            zoneId, skip, take, status, workspace, context.CancellationToken);

            //        if (response?.Success == true && response.Data != null)
            //        {
            //            return new List<ParkingSessionDataResponse> { response.Data };
            //        }

            //        return new List<ParkingSessionDataResponse>();
            //    }
            //);

            // Get parking sessions by access point
            //Field<ListGraphType<ParkingSessionType>>("parkingSessionsByAccessPoint")
            //    .Description("Get parking sessions by access point")
            //    .Arguments(new QueryArguments(
            //        new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "accessPointId", Description = "Access Point ID" },
            //        new QueryArgument<IntGraphType> { Name = "skip", Description = "Number of records to skip", DefaultValue = 0 },
            //        new QueryArgument<IntGraphType> { Name = "take", Description = "Number of records to take", DefaultValue = 20 },
            //        new QueryArgument<StringGraphType> { Name = "status", Description = "Filter by status" }
            //    ))
            //    .ResolveAsync(async context =>
            //    {
            //        var accessPointId = context.GetArgument<int>("accessPointId");
            //        var skip = context.GetArgument<int>("skip");
            //        var take = context.GetArgument<int>("take");
            //        var status = context.GetArgument<string?>("status");
            //        var workspace = "default";

            //        var response = await sessionService.GetParkingSessionsByAccessPointAsync(
            //            accessPointId, skip, take, status, workspace, context.CancellationToken);

            //        if (response?.Success == true && response.Data != null)
            //        {
            //            return new List<ParkingSessionDataResponse> { response.Data };
            //        }

            //        return new List<ParkingSessionDataResponse>();
            //    }
            //);

            // Get parking sessions by project area
            //Field<ListGraphType<ParkingSessionType>>("parkingSessionsByProjectArea")
            //    .Description("Get parking sessions by project area")
            //   .Arguments(new QueryArguments(
            //        new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "projectAreaId", Description = "Project Area ID" },
            //        new QueryArgument<IntGraphType> { Name = "skip", Description = "Number of records to skip", DefaultValue = 0 },
            //        new QueryArgument<IntGraphType> { Name = "take", Description = "Number of records to take", DefaultValue = 20 },
            //        new QueryArgument<StringGraphType> { Name = "status", Description = "Filter by status" }
            //    ))
            //   .ResolveAsync(async context =>
            //   {
            //       var projectAreaId = context.GetArgument<int>("projectAreaId");
            //       var skip = context.GetArgument<int>("skip");
            //       var take = context.GetArgument<int>("take");
            //       var status = context.GetArgument<string?>("status");
            //       var workspace = "default";

            //       var response = await sessionService.GetParkingSessionsByProjectAreaAsync(
            //           projectAreaId, skip, take, status, workspace, context.CancellationToken);

            //       if (response?.Success == true && response.Data != null)
            //       {
            //           return new List<ParkingSessionDataResponse> { response.Data };
            //       }

            //       return new List<ParkingSessionDataResponse>();
            //   }
            //);
        }
    }
}

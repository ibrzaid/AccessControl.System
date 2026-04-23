using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Models.Response.V1.ParkingService.Session;
using GraphQL.Types;
using System;
using System.Xml.Linq;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class SessionsResponseType : ObjectGraphType<SessionsResponse>
    {
        public SessionsResponseType()
        {
            Name = "SessionsResponse";
            Description = "Response containing parking sessions with pagination and filter information";

            Field(x => x.Success).Description("Indicates whether the request was successful");
            Field(x => x.Error, nullable: true).Description("Error message if the request failed");
            Field(x => x.ErrorCode, nullable: true) .Description("Error code if the request failed");

            // Sessions list
            Field<ListGraphType<ParkingSessionType>>("sessions")
                .Description("List of parking session data")
                .Resolve(context => context.Source.Sessions ?? []
            );

            // Pagination info
            Field<PaginationInfoResponseType>("pagination")
                .Description("Pagination information for the query results")
                .Resolve(context => context.Source.Pagination
            );

            
        }
    }
}

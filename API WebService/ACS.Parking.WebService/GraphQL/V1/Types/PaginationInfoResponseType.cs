using GraphQL.Types;
using ACS.Models.Response.V1.ParkingService.Session;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class PaginationInfoResponseType : ObjectGraphType<PaginationInfoResponse>
    {
        public PaginationInfoResponseType()
        {
            Name = "PaginationInfo";
            Description = "Pagination information for query results";

            Field(x => x.Total).Description("Total number of records matching the query");
            Field(x => x.Skip).Description("Number of records skipped (for pagination)");
            Field(x => x.Take).Description("Number of records taken in this response");
            Field(x => x.HasMore).Description("Indicates whether there are more records available after the current page");
        }
    }
}

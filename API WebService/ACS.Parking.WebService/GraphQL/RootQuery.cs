using GraphQL.Types;
using ACS.Parking.WebService.GraphQL.V1.Queries;

namespace ACS.Parking.WebService.GraphQL
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery(IServiceProvider serviceProvider)
        {
            Name = "Query";
            Description = "The root query for retrieving data.";
            var parkingSessionQuery = serviceProvider.GetRequiredService<ParkingSessionQuery>();
            foreach (var field in parkingSessionQuery.Fields)
                AddField(field);
        }
    }
}

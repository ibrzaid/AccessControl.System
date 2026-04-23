using GraphQL.Types;
using ACS.Master.WebService.GraphQL.V1.Queries;

namespace ACS.Master.WebService.GraphQL
{
    public class RootQuery: ObjectGraphType
    {
        public  RootQuery(IServiceProvider serviceProvider)
        {
            Name = "Query";
            Description = "The root query for retrieving data.";
            var countryQuery = serviceProvider.GetRequiredService<CountryQuery>();
            foreach (var field in countryQuery.Fields)
                AddField(field);
            

            // Merge PlateCategoryQuery fields
            var plateCategoryQuery = serviceProvider.GetRequiredService<PlateCategoryQuery>();
            foreach (var field in plateCategoryQuery.Fields)
                AddField(field);
            
        }
    }
}

using ACS.Master.WebService.GraphQL.V1.Queries;
using GraphQL.Types;

namespace ACS.Master.WebService.GraphQL
{
    public class AppSchema: Schema
    {
        public AppSchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<RootQuery>();
        }
    }
}

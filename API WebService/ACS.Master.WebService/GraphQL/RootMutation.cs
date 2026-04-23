using GraphQL.Types;

namespace ACS.Master.WebService.GraphQL
{
    public class RootMutation: ObjectGraphType
    {
        public RootMutation(ILogger<RootMutation> logger)
        {
            Name = "Mutation";
            Description = "The root Mutation for saving data.";
        }
    }
}

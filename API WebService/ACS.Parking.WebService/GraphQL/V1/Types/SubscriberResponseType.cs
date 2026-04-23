using GraphQL.Types;
using ACS.Models.Response.V1.SetupService;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class SubscriberResponseType : ObjectGraphType<SubscriberResponse>
    {
        public SubscriberResponseType()
        {
            Name = "Subscriber";
            Description = "Subscriber information";

            Field(x => x.SubscriberId).Description("Subscriber ID");
            Field(x => x.ProjectId, nullable: true).Description("Project ID");
            Field(x => x.SubscriberType, nullable: true).Description("Subscriber type");
            Field(x => x.Name, nullable: true).Description("Name");
            Field(x => x.ContactEmail, nullable: true).Description("Contact email");
            Field(x => x.ContactPhone, nullable: true).Description("Contact phone");
            Field(x => x.IsActive).Description("Is active");
            Field(x => x.CreatedAt).Description("Created at");
            Field(x => x.UpdatedAt).Description("Updated at");
        }
    }

}

using GraphQL.Types;
using ACS.Models.Response.V1.ParkingService.Entry;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class ParkingSessionType : ObjectGraphType<ParkingSessionDataResponse>
    {
        public ParkingSessionType()
        {
            Name = "ParkingSession";
            Description = "A parking session with all associated data";

            // ParkingSession fields
            Field<ParkingSessionResponseType>("parkingSession")
                .Resolve(context => context.Source.ParkingSession
            );

            // AccessPoint fields
            Field<AccessPointResponseType>("accessPoint")
                .Resolve(context => context.Source.AccessPoint
            );

            // Project fields
            Field<ProjectResponseType>( "project")
                .Resolve(context => context.Source.Project
            );

            // AreaZone fields
            Field<AreaZoneResponseType>("areaZone")
               .Resolve(context => context.Source.AreaZone
            );

            // Subscriber fields
            Field<SubscriberResponseType>("subscriber")
               .Resolve(context =>  context.Source.Subscriber
            );

            // Country fields
            Field<CountryResponseType>( "country")
                .Resolve(context =>  context.Source.Country
            );

            // PlateState fields
            Field<PlateStateResponseType>("plateState")
                .Resolve(context => context.Source.PlateState
            );

            // PlateCategory fields
            Field<PlateCategoryResponseType>("plateCategory")
               .Resolve(context => context.Source.PlateCategory
            );

            // User fields
            Field<UserProfileResponseType>("user")
                .Resolve(context => context.Source.User
            );

            // UserSession fields
            Field<UserSessionResponseType>("userSession")
                .Resolve(context => context.Source.UserSession
            );

            // Workspace fields
            Field<WorkspaceResponseType>( "workspace")
                .Resolve(context => context.Source.Workspace
            );
        }
    }
}

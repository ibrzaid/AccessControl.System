using ACS.BusinessEntities.AuthenticationService.V1.Authentication;
using ACS.BusinessEntities.MasterService.V1.County;
using ACS.BusinessEntities.MasterService.V1.PlateCategory;
using ACS.BusinessEntities.MasterService.V1.PlateState;
using ACS.BusinessEntities.ParkingService.V1.Entry;
using ACS.BusinessEntities.SetupService.V1;
using ACS.BusinessEntities.SubscriberService.V1;
using ACS.Models.Response.V1.AuthenticationService.Account;
using ACS.Models.Response.V1.MasterService.County;
using ACS.Models.Response.V1.MasterService.PlateCategory;
using ACS.Models.Response.V1.MasterService.PlateState;
using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Models.Response.V1.ParkingService.Session;
using ACS.Models.Response.V1.SetupService;

namespace ACS.Parking.WebService.Services.V1.Interfaces
{
    public interface IBaseService
    {
        SessionsResponse MapSessionsToResponseList(EntrySessionsEntity entity);
        ProjectResponse? MapProject(ProjectEntity? entity);
        AreaZoneResponse? MapAreaZone(AreaZoneEntity? entity);
        AccessPointResponse? MapAccessPoint(AccessPointEntity? entity);
        SubscriberResponse? MapSubscriber(SubscriberEntity? entity);
        CountryResponse? MapCountry(CountryEntity? entity);
        PlateStateResponse? MapPlateState(PlateStateEntity? entity);
        PlateCategoryResponse? MapPlateCategory(PlateCategoryEntity? entity);
        UserProfileResponse? MapUser(UserProfileEntity? entity);
        UserSessionResponse? MapUserSession(UserSessionEntity? entity);
        WorkspaceResponse? MapWorkspace(WorkspaceEntity? entity);
        MetadataResponse? MapMetadataToResponse(MetadataEntity? entity);
        ParkingSessionDataResponse? MapSessionDataToResponse(SessionDataEntity? entity);
    }
}

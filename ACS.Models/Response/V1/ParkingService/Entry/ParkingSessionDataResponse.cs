using System.Text.Json.Serialization;
using ACS.Models.Response.V1.SetupService;
using ACS.Models.Response.V1.MasterService.County;
using ACS.Models.Response.V1.MasterService.PlateState;
using ACS.Models.Response.V1.MasterService.PlateCategory;
using ACS.Models.Response.V1.AuthenticationService.Account;

namespace ACS.Models.Response.V1.ParkingService.Entry
{
    public class ParkingSessionDataResponse
    {
        [JsonPropertyName("parking_session")]
        public ParkingSessionResponse? ParkingSession { get; set; }

        [JsonPropertyName("access_point")]
        public AccessPointResponse? AccessPoint { get; set; }

        [JsonPropertyName("project")]
        public ProjectResponse? Project { get; set; }

        [JsonPropertyName("area_zone")]
        public AreaZoneResponse? AreaZone { get; set; }

        [JsonPropertyName("subscriber")]
        public SubscriberResponse? Subscriber { get; set; }

        [JsonPropertyName("country")]
        public CountryResponse? Country { get; set; }

        [JsonPropertyName("plate_state")]
        public PlateStateResponse? PlateState { get; set; }

        [JsonPropertyName("plate_category")]
        public PlateCategoryResponse? PlateCategory { get; set; }

        [JsonPropertyName("user")]
        public UserProfileResponse? User { get; set; }

        [JsonPropertyName("user_session")]
        public UserSessionResponse? UserSession { get; set; }

        [JsonPropertyName("workspace")]
        public WorkspaceResponse? Workspace { get; set; }
    }
}

using ACS.Models.Response.V1.ParkingService.Entry;


namespace ACS.Models.Response.V1.ParkingService.Session
{
    public class SessionsResponse : BaseResponse
    {
        public List<ParkingSessionDataResponse>? Sessions { get; set; }
        public PaginationInfoResponse? Pagination { get; set; }
    }
}

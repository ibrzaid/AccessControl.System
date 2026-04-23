

namespace ACS.Models.Response.V1.ParkingService.Session
{
    public class PaginationInfoResponse
    {
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool HasMore { get; set; }
    }
}

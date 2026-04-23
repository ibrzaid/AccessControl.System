using ACS.Models.Response.V1.MasterService.County;
using ACS.Models.Response.V1.MasterService.PlateCategoryDetail;

namespace ACS.Models.Response.V1.MasterService.PlateState
{
    public class PlateStateResponse
    {
        public int PlateStateId { get; set; }
        public Dictionary<string, string>? PlateStateName { get; set; }
        public int? CountryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public CountryResponse? Country { get; set; }
        public ICollection<PlateCategoryDetailRespones> PlateCategoryDetails { get; set; } = [];
    }
}

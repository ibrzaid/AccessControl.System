using ACS.Models.Response.V1.MasterService.County;
using ACS.Models.Response.V1.MasterService.PlateCategory;
using ACS.Models.Response.V1.MasterService.PlateState;

namespace ACS.Models.Response.V1.MasterService.PlateCategoryDetail
{
    public class PlateCategoryDetailRespones
    {
        public int PlateStateId { get; set; }
        public int CountryId { get; set; }
        public int PlateCategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public PlateStateResponse PlateState { get; set; } = default!;
        public CountryResponse Country { get; set; } = default!;
        public PlateCategoryResponse PlateCategory { get; set; } = default!;
    }
}

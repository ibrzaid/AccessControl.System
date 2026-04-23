using ACS.Models.Response.V1.MasterService.PlateCategoryDetail;

namespace ACS.Models.Response.V1.MasterService.PlateCategory
{
    public class PlateCategoryResponse
    {
        public int PlateCategoryId { get; set; }
        public string CategoryCode { get; set; } = default!;
        public Dictionary<string, string> CategoryNames { get; set; } = [];
        public Dictionary<string, string>? CategoryDescriptions { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public ICollection<PlateCategoryDetailRespones> PlateCategoryDetails { get; set; } = [];
    }
}

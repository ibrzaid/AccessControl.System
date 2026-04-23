using ACS.Models.Response.V1.MasterService.PlateCategoryDetail;
using ACS.Models.Response.V1.MasterService.PlateState;

namespace ACS.Models.Response.V1.MasterService.County
{
    public class CountryResponse
    {
        public int CountryId { get; set; }
        public string CountryCode { get; set; } = default!;
        public Dictionary<string, string> CountryNames { get; set; } = [];
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<PlateStateResponse> PlateStates { get; set; } = [];
        public ICollection<PlateCategoryDetailRespones> PlateCategoryDetails { get; set; } = [];

        public Dictionary<string, List<string>> Alphabets { get; set; } = [];
        public string Digits { get; set; } = "0123456789";
        public string PatternRegex { get; set; } = default!;
        public string PatternDescription { get; set; } = default!;
        public DateTime? PlateConfigCreatedAt { get; set; }
        public DateTime? PlateConfigUpdatedAt { get; set; }
    }
}
using ACS.Models.Response.V1.ANPRService.Seach;

namespace ACS.ANPR.WebService.Services.V1.Interfaces
{
    public interface ISearchService
    {
        Task<AnprSearchResponse> Search(string workspace,
            string requestId,
            string user,
            int? project,
            int? projectArea,
            long? zone,
            int? accessPoint,
            int? hardware,
            string? plateCode,
            string? plateNumber,
            DateTime? fromDate,
            DateTime? toDate,
            double? minConfidence,
            double? maxConfidence,
            int? country,
            int? state,
            int? category,
            string? direction,
            int? lane,
            string? vehicleType,
            string? vehicleColor,
            string? make,
            bool? validated,
            bool? blackListed,
            string? sortBy,
            string? sortDir,
            bool includeMapFeatures,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}

using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.ANPRService.V1;
using ACS.Models.Response.V1.ANPRService.Seach;
using ACS.ANPR.WebService.Services.V1.Interfaces;
using ACS.BusinessEntities.ANPRService.V1.Search;

namespace ACS.ANPR.WebService.Services.V1.Services
{
    public class SearchService(ILicenseManager licenseManager) : Service.Service(licenseManager), ISearchService
    {

        private ISearchDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.ANPRService.V1.SearchDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in SearchDataAccess.")
                };
            }
        }

        private static AnprSearchResponse Fail(string requestId, string code, string message) =>
            new(Success: false,
                Message: message,
                ErrorCode: code,
                RequestId: requestId,
                Data: null,
                Pagination: null,
                MapFeatures: null,
                Filters: null,
                Metadata: null);

        public async Task<AnprSearchResponse> Search(
            string workspace,
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
            CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].SearchAsync(
                    workspace: workspace,
                    user: user,
                    project: project,
                    projectArea: projectArea,
                    zone: zone,
                    accessPoint: accessPoint,
                    hardware: hardware,
                    plateCode: plateCode,
                    plateNumber: plateNumber,
                    fromDate: fromDate,
                    toDate: toDate,
                    minConfidence: minConfidence,
                    maxConfidence: maxConfidence,
                    country: country,
                    state: state,
                    category: category,
                    direction: direction,
                    lane: lane,
                    vehicleType: vehicleType,
                    vehicleColor: vehicleColor,
                    make: make,
                    validated: validated,
                    blackListed: blackListed,
                    sortBy: sortBy,
                    sortDir: sortDir,
                    includeMapFeatures: includeMapFeatures,
                    page: page,
                    pageSize: pageSize,
                    cancellationToken: cancellationToken);
            if (!result.Success) return Fail(requestId, result.ErrorCode ?? "SEARCH_FAILED", result.Message ?? "Search failed");


            return new AnprSearchResponse(
                Success: true,
                Message: result.Message,
                ErrorCode: result.ErrorCode ?? "",
                RequestId: requestId,

                Pagination: result.Pagination is null ? null : new AnprSearchPaginationResponse(
                    Page: result.Pagination.Page,
                    PageSize: result.Pagination.PageSize,
                    TotalCount: result.Pagination.TotalCount,
                    TotalPages: result.Pagination.TotalPages),

                Data: result.Data is null ? [] :
                    [.. result.Data.Select(d => MapDetection(d))],

                MapFeatures: result.MapFeatures is null ? null : MapFeatures(result.MapFeatures),

                Filters: result.Filters is null ? null : new AnprSearchFiltersResponse(
                    WorkspaceId: result.Filters.WorkspaceId,
                    UserId: result.Filters.UserId,
                    ProjectId: result.Filters.ProjectId,
                    ProjectAreaId: result.Filters.ProjectAreaId,
                    ZoneId: result.Filters.ZoneId,
                    AccessPointId: result.Filters.AccessPointId,
                    HardwareId: result.Filters.HardwareId,
                    PlateCode: result.Filters.PlateCode,
                    PlateNumber: result.Filters.PlateNumber,
                    FromDate: result.Filters.FromDate,
                    ToDate: result.Filters.ToDate,
                    MinConfidence: result.Filters.MinConfidence,
                    MaxConfidence: result.Filters.MaxConfidence,
                    CountryId: result.Filters.CountryId,
                    StateId: result.Filters.StateId,
                    CategoryId: result.Filters.CategoryId,
                    Direction: result.Filters.Direction,
                    LaneNumber: result.Filters.LaneNumber,
                    VehicleType: result.Filters.VehicleType,
                    VehicleColor: result.Filters.VehicleColor,
                    VehicleMake: result.Filters.VehicleMake,
                    IsValidated: result.Filters.IsValidated,
                    IsBlacklisted: result.Filters.IsBlacklisted,
                    SortBy: result.Filters.SortBy,
                    SortDir: result.Filters.SortDir,
                    IncludeMapFeatures: result.Filters.IncludeMapFeatures),
                Metadata: result.Metadata is null ? null : new AnprSearchMetadataResponse(
                    Timezone: result.Metadata.Timezone,
                    ProcessingTimeMs: result.Metadata.ProcessingTimeMs,
                    Timestamp: result.Metadata.Timestamp)
            );




        }

        private static AnprSearchDetectionResponse MapDetection(AnprSearchDetectionEntity d) =>
            new(DetectionId: d.DetectionId,
                DetectionTime: d.DetectionTime,
                DetectionTimeUtc: d.DetectionTimeUtc,
                AgeSeconds: d.AgeSeconds,
                PlateCode: d.PlateCode,
                PlateNumber: d.PlateNumber,
                FullPlate: d.FullPlate,
                ConfidenceScore: d.ConfidenceScore,
                ConfidenceDetails: d.ConfidenceDetails,      
                ImageUrl: d.ImageUrl,
                PlateCropUrl: d.PlateCropUrl,
                VehicleColor: d.VehicleColor,
                VehicleMake: d.VehicleMake,
                VehicleModel: d.VehicleModel,
                VehicleType: d.VehicleType,
                VehicleSpeedKmh: d.VehicleSpeedKmh,
                Direction: d.Direction,
                LaneNumber: d.LaneNumber,
                Latitude: d.Latitude,
                Longitude: d.Longitude,
                AnprEngine: d.AnprEngine,
                ProcessingTimeMs: d.ProcessingTimeMs,
                IsValidated: d.IsValidated,
                ValidatedAt: d.ValidatedAt,
                ValidatedAtUtc: d.ValidatedAtUtc,
                ValidatedBy: d.ValidatedBy,           
                ValidationNotes: d.ValidationNotes,
                Tags: d.Tags,
                CreatedAt: d.CreatedAt,
                IsBlacklisted: d.IsBlacklisted,
                BlacklistDetails: d.BlacklistDetails is null ? null : new AnprSearchBlacklistResponse(
                    ListType: d.BlacklistDetails.ListType,
                    Reason: d.BlacklistDetails.Reason,
                    Severity: d.BlacklistDetails.Severity,
                    ActionOnDetect: d.BlacklistDetails.ActionOnDetect,
                    ReferenceNumber: d.BlacklistDetails.ReferenceNumber),
                Hardware: d.Hardware is null ? null : new AnprSearchHardwareResponse(
                    Id: d.Hardware.Id,
                    SerialNumber: d.Hardware.SerialNumber,
                    Manufacturer: d.Hardware.Manufacturer,
                    Model: d.Hardware.Model,
                    HardwareType: d.Hardware.HardwareType,
                    Status: d.Hardware.Status),
                AccessPoint: d.AccessPoint is null ? null : new AnprSearchAccessPointResponse(
                    Id: d.AccessPoint.Id,
                    Name: d.AccessPoint.Name,
                    Prefix: d.AccessPoint.Prefix,
                    Latitude: d.AccessPoint.Latitude,
                    Longitude: d.AccessPoint.Longitude,
                    Type: d.AccessPoint.Type,
                    Orientation: d.AccessPoint.Orientation),
                Project: d.Project is null ? null : new AnprSearchNamedRefResponse(d.Project.Id, d.Project.Name),
                Area: d.Area is null ? null : new AnprSearchNamedRefResponse(d.Area.Id, d.Area.Name),
                Zone: d.Zone is null ? null : new AnprSearchZoneResponse(d.Zone.Id, d.Zone.Code, d.Zone.Name),
                Country: d.Country is null ? null : new AnprSearchCountryResponse(d.Country.Id, d.Country.Code, d.Country.Name),
                State: d.State is null ? null : new AnprSearchNamedRefResponse(d.State.Id, d.State.Name),
                Category: d.Category is null ? null : new AnprSearchNamedRefResponse(d.Category.Id, d.Category.Name),
                ValidatedByUser: d.ValidatedByUser is null ? null : new AnprSearchUserResponse(
                    Id: d.ValidatedByUser.Id,
                    Username: d.ValidatedByUser.Username,
                    FullName: d.ValidatedByUser.FullName));

        private static AnprSearchMapFeaturesResponse MapFeatures(AnprSearchMapFeaturesEntity mf) =>
            new(Type: mf.Type,
                IsSinglePlate: mf.IsSinglePlate,
                HasPath: mf.HasPath,
                Features: mf.Features is null ? [] :
                    [.. mf.Features.Select(f => new AnprSearchFeatureResponse(
                        Type:     f.Type,
                        Geometry: f.Geometry is null ? null : new AnprSearchGeometryResponse(
                            Type:        f.Geometry.Type,
                            Coordinates: f.Geometry.Coordinates),
                        Properties: f.Properties is null ? null : new AnprSearchFeaturePropertiesResponse(
                            Type:          f.Properties.Type,
                            MarkerColor:   f.Properties.MarkerColor,
                            Id:            f.Properties.Id,
                            Time:          f.Properties.Time,
                            AgeSeconds:    f.Properties.AgeSeconds,
                            Plate:         f.Properties.Plate,
                            PlateCode:     f.Properties.PlateCode,
                            PlateNumber:   f.Properties.PlateNumber,
                            Confidence:    f.Properties.Confidence,
                            Direction:     f.Properties.Direction,
                            LaneNumber:    f.Properties.LaneNumber,
                            IsBlacklisted: f.Properties.IsBlacklisted,
                            IsValidated:   f.Properties.IsValidated,
                            ImageUrl:      f.Properties.ImageUrl,
                            PlateCropUrl:  f.Properties.PlateCropUrl,
                            VehicleColor:  f.Properties.VehicleColor,
                            VehicleType:   f.Properties.VehicleType,
                            SpeedKmh:      f.Properties.SpeedKmh,
                            BlSeverity:    f.Properties.BlSeverity,
                            BlReason:      f.Properties.BlReason,
                            StartTime:     f.Properties.StartTime,
                            EndTime:       f.Properties.EndTime,
                            TotalStops:    f.Properties.TotalStops,
                            Name:          f.Properties.Name,
                            Prefix:        f.Properties.Prefix,
                            TypeCode:      f.Properties.TypeCode,
                            Orientation:   f.Properties.Orientation,
                            VisitCount:    f.Properties.VisitCount,
                            FirstSeen:     f.Properties.FirstSeen,
                            LastSeen:      f.Properties.LastSeen,
                            AvgConfidence: f.Properties.AvgConfidence,
                            AccessPoint:   f.Properties.AccessPoint is null ? null
                                : new AnprSearchFeatureAccessPointResponse(   
                                    Id:          f.Properties.AccessPoint.Id,
                                    Name:        f.Properties.AccessPoint.Name,
                                    Prefix:      f.Properties.AccessPoint.Prefix,
                                    Type:        f.Properties.AccessPoint.Type,
                                    Orientation: f.Properties.AccessPoint.Orientation))))]);
    }
}

using ACS.License.V1;
using System.Text.Json;
using ACS.Service.V1.Interfaces;
using ACS.Models.Response.V1.ANPRService.Live;
using ACS.Database.IDataAccess.ANPRService.V1;
using ACS.ANPR.WebService.Services.V1.Interfaces;
using ACS.BusinessEntities.ANPRService.V1.Live.Request;
using ACS.BusinessEntities.ANPRService.V1.Live.Responses;

namespace ACS.ANPR.WebService.Services.V1.Services
{
    public class LiveService(ILicenseManager licenseManager,  ILogger<LiveService> logger) : Service.Service(licenseManager) , ILiveService
    {
        private static readonly JsonSerializerOptions _opts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private ILiveDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.ANPRService.V1.LiveDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in MAPService.")
                };
            }
        }

        public async Task<LiveResponse> GetMapViewAsync(
             string viewType,
             int? projectId,
             int? areaId,
             long? zoneId,
             int? accessPointId,
             int? hardwareId,
             int zoomLevel,
             double? centerLat,
             double? centerLng,
             double? boundsSouth,
             double? boundsNorth,
             double? boundsWest,
             double? boundsEast,
             DateTime? fromTime,
             DateTime? toTime,
             int windowMinutes,
             double minConfidence,
             int resolution,
             int clusterRadius,
             bool weightByConfidence,
             bool includeTrend,
             string? plateCode,
             string? plateNumber,
             bool includePath,
             bool includeStops,
             double heightFactor,
             int limit,
             int offset,
             string workspaceId,
             string? user,
             string? ipAddress,
             string? agent,
             string? requestId,
             CancellationToken cancellationToken = default)
        {
            try
            {
                // ── Assemble bounds if all four edges supplied ──────────────────
                BoundsRequest? bounds = null;
                if (boundsSouth.HasValue && boundsNorth.HasValue &&
                    boundsWest.HasValue  && boundsEast.HasValue)
                {
                    bounds = new BoundsRequest(
                        South: boundsSouth.Value,
                        North: boundsNorth.Value,
                        West: boundsWest.Value,
                        East: boundsEast.Value,
                        CenterLat: centerLat,
                        CenterLng: centerLng);
                }

                var request = new MapDataRequest(
                    WorkspaceId: int.Parse(workspaceId),
                    ViewType: viewType,
                    ProjectId: projectId,
                    ProjectAreaId: areaId,
                    ZoneId: zoneId,
                    AccessPointId: accessPointId,
                    HardwareId: hardwareId,
                    Bounds: bounds,
                    CenterLat: centerLat,
                    CenterLng: centerLng,
                    ZoomLevel: zoomLevel,
                    FromTime: fromTime,
                    ToTime: toTime,
                    WindowMinutes: windowMinutes,
                    MinConfidence: minConfidence,
                    Resolution: resolution,
                    ClusterRadius: clusterRadius,
                    WeightByConfidence: weightByConfidence,
                    IncludeTrend: includeTrend,
                    PlateCode: plateCode,
                    PlateNumber: plateNumber,
                    IncludePath: includePath,
                    IncludeStops: includeStops,
                    HeightFactor: heightFactor,
                    Limit: limit,
                    Offset: offset);

                var license = this.LicenseManager.GetLicense();
                var result = await this[license?.DB!].GetMapViewAsync(
                    request, user, ipAddress, agent, requestId,
                    latitude: 0, longitude: 0, cancellationToken);

                if (result is null)
                    return Error(requestId, "NO_RESULT", "No result from data access layer.");

                if (!result.Success)
                    return Error(requestId, result.ErrorCode, result.Message);

                return MapResult(result, requestId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MapService.GetMapViewAsync failed. RequestId={RequestId}", requestId);
                return Error(requestId, "INTERNAL_ERROR", ex.Message);
            }
        }


        // ── Map entity → response ──────────────────────────────────────────────
        private static LiveResponse MapResult(MapDataResponse<object> r, string? requestId)
        {
            // ── breadcrumb
            var breadcrumb = r.Breadcrumb?.Select(b => new MapBreadcrumbItem(
                Id: b.Id,
                Name: b.Name is not null ? JsonSerializer.SerializeToElement(b.Name, _opts) : null,
                Code: b.Code,
                Type: b.Type)).ToList();

            // ── hierarchy ids
            var hierarchyIds = r.HierarchyIds is null ? null : new MapHierarchyIdsResponse(
                WorkspaceId: r.HierarchyIds.WorkspaceId,
                ProjectId: r.HierarchyIds.ProjectId,
                ProjectAreaId: r.HierarchyIds.ProjectAreaId,
                ZoneId: r.HierarchyIds.ZoneId,
                AccessPointId: r.HierarchyIds.AccessPointId,
                HardwareId: r.HierarchyIds.HardwareId);

            // ── master
            var master = r.Master is null ? null : new MapMasterResponse(
                RequestedView: r.Master.RequestedView,
                EffectiveView: r.Master.EffectiveView,
                FunctionCalled: r.Master.FunctionCalled,
                ProcessingTimeMs: r.Master.ProcessingTimeMs,
                AuditLogId: r.Master.AuditLogId,
                Timestamp: r.Master.Timestamp);

            // ── metadata
            var metadata = r.Metadata is null ? null : new MapMetadataResponse(
                ProcessingTimeMs: r.Metadata.ProcessingTimeMs,
                Timestamp: r.Metadata.Timestamp);

            // ── bounds  (all views use flat center_lat/center_lng)
            var bounds = r.Bounds is null ? null : new MapBoundsResponse(
                South: r.Bounds.South,
                North: r.Bounds.North,
                West: r.Bounds.West,
                East: r.Bounds.East,
                CenterLat: r.Bounds.CenterLat,
                CenterLng: r.Bounds.CenterLng,
                Zoom: r.Bounds.Zoom);

            // ── pagination (live + clusters)
            var pagination = r.Pagination is null ? null : new MapPaginationResponse(
                Total: r.Pagination.Total,
                Limit: r.Pagination.Limit,
                Offset: r.Pagination.Offset,
                HasMore: r.Pagination.HasMore);

            // ── zone_info (zone_children level)
            // DB returns name as single en-US string — pass through directly
            var zoneInfo = r.ZoneInfo is null ? null : new MapZoneInfoResponse(
                Id: r.ZoneInfo.Id,
                Name: r.ZoneInfo.Name,   // already string? from zone_names->>'en-US'
                Code: r.ZoneInfo.Code,
                TotalSpots: r.ZoneInfo.TotalSpots,
                AvailableSpots: r.ZoneInfo.AvailableSpots,
                CenterLat: r.ZoneInfo.CenterLat,
                CenterLng: r.ZoneInfo.CenterLng);

            // ── access_point info (access_point level)
            // names/type_names are jsonb dicts — pass through as JsonElement
            MapAccessPointInfoResponse? accessPoint = null;
            if (r.AccessPoint is not null)
            {
                var ap = r.AccessPoint;
                accessPoint = new MapAccessPointInfoResponse(
                    Id: ap.Id,
                    Names: ap.Names is not null
                                   ? JsonSerializer.SerializeToElement(ap.Names, _opts) : null,
                    Prefix: ap.Prefix,
                    TypeCode: ap.TypeCode,
                    TypeNames: ap.TypeNames is not null
                                   ? JsonSerializer.SerializeToElement(ap.TypeNames, _opts) : null,
                    Coordinates: ap.Coordinates is null ? null
                                   : new MapCoordinateResponse(ap.Coordinates.Lat, ap.Coordinates.Lng),
                    Orientation: ap.Orientation,
                    PositionX: ap.PositionX,
                    PositionY: ap.PositionY,
                    IsActive: ap.IsActive,
                    HardwareCount: ap.HardwareCount,
                    Detections24h: ap.Detections24h);
            }

            // ── hardware info (hardware level)
            // type_names/status_names = jsonb dicts → JsonElement
            // configuration/location/performance_24h = arbitrary jsonb → typed records
            MapHardwareInfoResponse? hardware = null;
            if (r.Hardware is not null)
            {
                var hw = r.Hardware;
                hardware = new MapHardwareInfoResponse(
                    Id: hw.Id,
                    Type: hw.Type,
                    TypeNames: hw.TypeNames is not null
                                     ? JsonSerializer.SerializeToElement(hw.TypeNames, _opts) : null,
                    Manufacturer: hw.Manufacturer,
                    Model: hw.Model,
                    SerialNumber: hw.SerialNumber,
                    FirmwareVersion: hw.FirmwareVersion,
                    Status: hw.Status,
                    StatusNames: hw.StatusNames is not null
                                     ? JsonSerializer.SerializeToElement(hw.StatusNames, _opts) : null,
                    LastMaintenance: hw.LastMaintenance,
                    NextMaintenance: hw.NextMaintenance,
                    Configuration: hw.Configuration,          // already JsonElement?
                    LastActivity: hw.LastActivity,
                    DetectionCount: hw.DetectionCount,
                    IsActive: hw.IsActive,
                    CreatedAt: hw.CreatedAt,
                    Location: hw.Location is null ? null
                                     : new MapHardwareLocationResponse(
                                           Lat: hw.Location.Lat,
                                           Lng: hw.Location.Lng,
                                           AccessPoints: hw.Location.AccessPoints,  // JsonElement?
                                           Zone: hw.Location.Zone,
                                           Area: hw.Location.Area,
                                           Project: hw.Location.Project),
                    Performance24h: hw.Performance24h is null ? null
                                     : new MapHardwarePerformanceResponse(
                                           Total: hw.Performance24h.Total,
                                           AvgConfidence: hw.Performance24h.AvgConfidence,
                                           PeakHour: hw.Performance24h.PeakHour));
            }

            // ── heatmap data cells
            var heatmapData = r.Data?.Select(d => new MapHeatmapCellResponse(
                X: d.X,
                Y: d.Y,
                Lat: d.Latitude,
                Lng: d.Longitude,
                Count: d.Count,
                Weight: d.Weight,
                Intensity: d.Intensity,
                AvgConfidence: d.AvgConfidence,
                UniquePlates: d.UniquePlates,
                LastDetection: d.LastDetection,
                IsHotspot: d.IsHotspot,
                DominantColor: d.DominantColor)).ToList();

            // ── hotspots (live_heatmap only)
            var hotspots = r.Hotspots?.Select(h => new MapHotspotResponse(
                Lat: h.Latitude,
                Lng: h.Longitude,
                Intensity: h.Intensity,
                Rank: h.Rank)).ToList();

            // ── live_config (live_heatmap only)
            var liveConfig = r.LiveConfig is null ? null : new MapLiveConfigResponse(
                WindowMinutes: r.LiveConfig.WindowMinutes,
                RefreshIntervalSeconds: r.LiveConfig.RefreshIntervalSeconds,
                NextUpdate: r.LiveConfig.NextUpdate,
                Resolution: r.LiveConfig.Resolution,
                TotalCells: r.LiveConfig.TotalCells);

            // ── 3D camera
            var camera = r.Camera is null ? null : new MapCameraConfigResponse(
                Position: new MapCameraPositionResponse(
                    Lat: r.Camera.Position.Latitude,
                    Lng: r.Camera.Position.Longitude,
                    Altitude: r.Camera.Position.Altitude,
                    Pitch: r.Camera.Position.Pitch,
                    Bearing: r.Camera.Position.Bearing),
                MaxPitch: r.Camera.MaxPitch,
                MinPitch: r.Camera.MinPitch);

            // ── satellite center {lat,lng}
            var center = r.Center is null ? null
                         : new MapCoordinateResponse(r.Center.Latitude, r.Center.Longitude);

            return new LiveResponse(
                Success: true,
                Message: r.Message ?? "",
                ErrorCode: "",
                RequestId: requestId ?? "",
                CurrentLevel: r.CurrentLevel,
                AvailableViews: r.AvailableViews,
                Breadcrumb: breadcrumb,
                HierarchyIds: hierarchyIds,
                Master: master,
                Type: r.Type,
                Features: r.Features,
                Total: r.Total,
                Bounds: bounds,
                Data: heatmapData,
                Hotspots: hotspots,
                LiveConfig: liveConfig,
                Stats: r.Stats is JsonElement statsEl ? statsEl : null,
                Pagination: pagination,
                Timestamp: r.Timestamp,
                RefreshInterval: r.RefreshInterval,
                ZoneInfo: zoneInfo,
                AccessPoint: accessPoint,
                TotalHardware: r.TotalHardware,
                Hardware: hardware,
                RecentDetections: r.RecentDetections,
                Camera: camera,
                Center: center,
                Radius: r.Radius,
                Filters: r.Filters is not null
                                  ? JsonSerializer.SerializeToElement(r.Filters, _opts) : null,
                Metadata: metadata);
        }


        // ── Error helper ──────────────────────────────────────────────────────
        private static LiveResponse Error(string? requestId, string? code, string? msg) => new(
            Success: false,
            Message: msg  ?? "",
            ErrorCode: code ?? "ERROR",
            RequestId: requestId ?? "",
            CurrentLevel: null, AvailableViews: null,
            Breadcrumb: null, HierarchyIds: null, Master: null,
            Type: null, Features: null, Total: null,
            Bounds: null,
            Data: null, Hotspots: null, LiveConfig: null,
            Stats: null,
            Pagination: null, Timestamp: null, RefreshInterval: null,
            ZoneInfo: null, AccessPoint: null, TotalHardware: null,
            Hardware: null, RecentDetections: null,
            Camera: null, Center: null, Radius: null,
            Filters: null, Metadata: null);
    }
}
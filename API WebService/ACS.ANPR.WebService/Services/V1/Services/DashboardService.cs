using ACS.Helper;
using ACS.Background;
using ACS.License.V1;
using System.Text.Json;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.ANPRService.V1;
using ACS.ANPR.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.ANPRService.Dashboard;

namespace ACS.ANPR.WebService.Services.V1.Services
{
    public class DashboardService(ILicenseManager licenseManager, IBackgroundTaskQueue backgroundTaskQueue, INotificationService notificationService, ILogger<DashboardService> logger) : Service.Service(licenseManager), IDashboardService
    {
        private IDashboardDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.ANPRService.V1.DashboardDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in DashoardService.")
                };
            }
        }

        public void StartNotify()
        {
            backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                try
                {
                    var license = this.LicenseManager.GetLicense();
                    this[license?.DB!].DashboardChanged+=(object? sender, JsonElement e) =>
                    {
                        var ev = e.Str("event");
                        var workspaceId = e.IntN("workspace_id");
                        var projectId = e.IntN("project_id");
                        var userId = e.IntN("user_id");
                        var data = e.TryGetProperty("data", out var d) ? d : default;
                        if (workspaceId== null) return;
                        switch (ev)
                        {
                            case "hourly_bucket_updated":
                                object payload = new
                                {
                                    hour = data.Str("hour"),
                                    detections = data.Int("detections"),
                                    blacklist = data.Int("blacklist"),
                                    unique_plates = data.Int("unique_plates"),
                                    avg_confidence = data.DblN("avg_confidence"),
                                    workspace_id = workspaceId,
                                    project_id = projectId,
                                };
                                notificationService.SendToWorkspaceGroup(ev, workspaceId.ToString()!, payload);
                                if (projectId.HasValue) notificationService.SendToProjectGroup(ev, workspaceId?.ToString()!, projectId.Value.ToString(), payload);
                                break;
                            case "daily_stats_updated":
                                payload = new
                                {
                                    summary_date = data.Str("summary_date"),
                                    total_detections = data.IntN("total_detections"),
                                    unique_plates = data.IntN("unique_plates"),
                                    avg_confidence = data.DblN("avg_confidence"),
                                    blacklist_hits = data.IntN("blacklist_hits"),
                                    whitelist_hits = data.IntN("whitelist_hits"),
                                    high_confidence = data.IntN("high_confidence"),
                                    low_confidence = data.IntN("low_confidence"),
                                    peak_hour = data.IntN("peak_hour"),
                                    peak_hour_count = data.IntN("peak_hour_count"),
                                    active_cameras = data.IntN("active_cameras"),
                                    offline_cameras = data.IntN("offline_cameras"),
                                    workspace_id = workspaceId,
                                    project_id = projectId,
                                };
                                notificationService.SendToWorkspaceGroup(ev, workspaceId.ToString()!, payload);
                                if (projectId.HasValue) notificationService.SendToProjectGroup(ev, workspaceId?.ToString()!, projectId.Value.ToString(), payload);
                                break;

                            case "hotspot_updated":
                                payload = new
                                {
                                    access_point_id = data.IntN("access_point_id"),
                                    access_point_name = data.Str("access_point_name"),
                                    prefix = data.Str("prefix"),
                                    latitude = data.DblN("latitude"),
                                    longitude = data.DblN("longitude"),
                                    summary_date = data.Str("summary_date"),
                                    total_detections = data.IntN("total_detections"),
                                    unique_plates = data.IntN("unique_plates"),
                                    blacklist_hits = data.IntN("blacklist_hits"),
                                    avg_confidence = data.DblN("avg_confidence"),
                                    peak_hour = data.IntN("peak_hour"),
                                    peak_hour_count = data.IntN("peak_hour_count"),
                                    workspace_id = workspaceId,
                                    project_id = projectId,
                                };
                                notificationService.SendToWorkspaceGroup(ev, workspaceId.ToString()!, payload);
                                if (projectId.HasValue)notificationService.SendToProjectGroup(ev, workspaceId.ToString()!, projectId.Value.ToString(), payload);
                                break;

                        }
                    };
                    await this[license?.DB!].StartNotify(token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while listening to dashoard notification.");
                }
            });
        }


        public async Task<AnprDashboardResponse> GetDashboardAsync(string wrokspace, string user, string requestId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDashboardAsync(wrokspace, user, cancellationToken);
            return new(
                Success: result.Success,
                Message: result.Message,
                ErrorCode: "",
                RequestId: requestId,
                GeneratedAt: result.GeneratedAt,
                Detail: result.Detail,
                Module: result.Module,
                Enabled: result.Enabled,
                Anpr: result.Anpr == null ? null : new AnprStatsResponse(
                     DetectionsToday: result.Anpr.DetectionsToday,
                     DetectionsYesterday: result.Anpr.DetectionsYesterday,
                     DetectionsDeltaPct: result.Anpr.DetectionsDeltaPct,
                     UniquePlatesToday: result.Anpr.UniquePlatesToday,
                     BlacklistHitsToday: result.Anpr.BlacklistHitsToday,
                     AvgConfidence: result.Anpr.AvgConfidence,
                     HighConfidence: result.Anpr.HighConfidence,
                     LowConfidence: result.Anpr.LowConfidence,
                     PeakHour: result.Anpr.PeakHour,
                     PeakHourCount: result.Anpr.PeakHourCount
                    ),
                HourlyTrend: result.HourlyTrend == null ? [] : [.. result.HourlyTrend.Select(trend => new HourlyBucketResponse(
                    Hour: trend.Hour,
                    Detections: trend.Detections,
                    Blacklist: trend.Blacklist
                    ))],
                 ProjectsDetections: result.ProjectsDetections == null ? [] : [..result.ProjectsDetections.Select( detaction =>
                 new ProjectDetectionResponse(
                     ProjectId: detaction.ProjectId,
                     DetectionsToday: detaction.DetectionsToday
                     ))]

                );
        }
       
        public async Task<AnprDashboardProjectResponse> GetDashboardByProjectAsync(string workspaceId, int projectId, string userId, string requestId, string viewType = "daily", int days = 30, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].GetDashboardByProjectAsync(workspaceId, projectId, userId, viewType, days, cancellationToken);

            return new AnprDashboardProjectResponse(
        Success: result.Success,
        Message: result.Message,
        ErrorCode: result.ErrorCode ?? "",
        RequestId: requestId,

        Timestamp: result.Timestamp,
        WorkspaceId: result.WorkspaceId,
        ProjectId: result.ProjectId,
        Scope: result.Scope,
        ViewType: result.ViewType,
        Timezone: result.Timezone,

        Metadata: result.Metadata is null ? null : new AnprMetadataResponse(
            ProcessingTimeMs: result.Metadata.ProcessingTimeMs),

        SelectedPeriod: result.SelectedPeriod is null ? null : new AnprSelectedPeriodResponse(
            Type: result.SelectedPeriod.Type,
            Label: result.SelectedPeriod.Label,
            CompareLabel: result.SelectedPeriod.CompareLabel,
            From: result.SelectedPeriod.From,
            To: result.SelectedPeriod.To,
            WeekStart: result.SelectedPeriod.WeekStart,
            WeekEnd: result.SelectedPeriod.WeekEnd,
            MonthStart: result.SelectedPeriod.MonthStart,
            MonthEnd: result.SelectedPeriod.MonthEnd,
            MonthName: result.SelectedPeriod.MonthName),

        Kpis: result.Kpis is null ? null : new AnprKpisResponse(
            Detections: new AnprDetectionKpiResponse(
                Today: result.Kpis.Detections.Today,
                Yesterday: result.Kpis.Detections.Yesterday,
                ThisWeek: result.Kpis.Detections.ThisWeek,
                ThisMonth: result.Kpis.Detections.ThisMonth,
                ChangePct: result.Kpis.Detections.ChangePct),
            UniquePlates: new AnprSingleCountResponse(Today: result.Kpis.UniquePlates.Today),
            Blacklist: new AnprSingleCountResponse(Today: result.Kpis.Blacklist.Today),
            Whitelist: new AnprSingleCountResponse(Today: result.Kpis.Whitelist.Today),
            Confidence: new AnprConfidenceKpiResponse(TodayAvg: result.Kpis.Confidence.TodayAvg),
            PeakHour: new AnprPeakHourResponse(
                Hour: result.Kpis.PeakHour.Hour,
                Count: result.Kpis.PeakHour.Count)),

        HourlyTrend: result.HourlyTrend is null ? [] :
            [.. result.HourlyTrend.Select(h => new AnprHourlyTrendResponse(
                Hour:          h.Hour,
                Detections:    h.Detections,
                UniquePlates:  h.UniquePlates,
                Blacklist:     h.Blacklist,
                AvgConfidence: h.AvgConfidence))],

        DailyTrend: result.DailyTrend is null ? [] :
            [.. result.DailyTrend.Select(d => new AnprDailyTrendResponse(
                Date:          d.Date,
                DayName:       d.DayName,
                DayOfWeek:     d.DayOfWeek,
                DayOfMonth:    d.DayOfMonth,
                Detections:    d.Detections,
                UniquePlates:  d.UniquePlates,
                Blacklist:     d.Blacklist,
                AvgConfidence: d.AvgConfidence))],

        Comparison: result.Comparison is null ? null : new AnprComparisonResponse(
            Current: result.Comparison.Current,
            Previous: result.Comparison.Previous,
            ChangePct: result.Comparison.ChangePct),

        VehicleDistribution: result.VehicleDistribution is null ? [] :
            [.. result.VehicleDistribution.Select(v => new AnprVehicleTypeResponse(
                VehicleType: v.VehicleType,
                Count:       v.Count,
                Percentage:  v.Percentage))],

        // ── Hotspots ─────────────────────────────────────────────────────────
        Hotspots: result.Hotspots is null ? [] :
            [.. result.Hotspots.Select(h => new AnprHotspotResponse(
                AccessPointId:   h.AccessPointId,
                AccessPointName: h.AccessPointName,
                Prefix:          h.Prefix,
                Latitude:        h.Latitude,
                Longitude:       h.Longitude,
                IsActive:        h.IsActive,
                Detections:      h.Detections,
                UniquePlates:    h.UniquePlates,
                BlacklistHits:   h.BlacklistHits,
                AvgConfidence:   h.AvgConfidence,
                PeakHour:        h.PeakHour,
                ChangePct:       h.ChangePct))],

        // ── System / cameras ─────────────────────────────────────────────────
        System: result.System is null ? null : new AnprSystemResponse(
            Cameras: result.System.Cameras is null ? null : new AnprCameraCountsResponse(
                Total: result.System.Cameras.Total,
                Online: result.System.Cameras.Online,
                Offline: result.System.Cameras.Offline,
                Warning: result.System.Cameras.Warning,
                Error: result.System.Cameras.Error,
                OnlinePct: result.System.Cameras.OnlinePct,
                UptimePct: result.System.Cameras.UptimePct),
            CameraList: result.System.CameraList is null ? [] :
                [.. result.System.CameraList.Select(c => new CameraListItemResponse(
                    HardwareId:            c.HardwareId,
                    HardwareLabel:         c.HardwareLabel,
                    SerialNumber:          c.SerialNumber,
                    AccessPointId:         c.AccessPointId,
                    AccessPointName:       c.AccessPointName,
                    StatusId:              c.StatusId,
                    IsActive:              c.IsActive,
                    LastActivity:          c.LastActivity,
                    LastDetectionTime:     c.LastDetectionTime,
                    MinutesSinceDetection: c.MinutesSinceDetection))]),

        // ── Plate hits ───────────────────────────────────────────────────────
        PlateHits: result.PlateHits is null ? null : new AnprPlateHitsResponse(
            BlacklistToday: result.PlateHits.BlacklistToday,
            WhitelistToday: result.PlateHits.WhitelistToday,
            GraylistToday: result.PlateHits.GraylistToday,
            RecentHits: result.PlateHits.RecentHits is null ? [] :
                [.. result.PlateHits.RecentHits.Select(h => new AnprHitResponse(
                    HitId:         h.HitId,
                    HitTime:       h.HitTime,
                    ListType:      h.ListType,
                    Severity:      h.Severity,
                    PlateCode:     h.PlateCode,
                    PlateNumber:   h.PlateNumber,
                    FullPlate:     h.FullPlate,
                    Reason:        h.Reason,
                    DetectionId:   h.DetectionId,
                    AccessPointId: h.AccessPointId,
                    ActionTaken:   h.ActionTaken,
                    Resolved:      h.Resolved))]),

        // ── Recent detections ────────────────────────────────────────────────
        RecentDetections: result.RecentDetections is null ? null : new AnprRecentDetectionsWrapperResponse(
            Success: result.RecentDetections.Success,
            Message: result.RecentDetections.Message,
            Metadata: result.RecentDetections.Metadata is null ? null : new AnprRecentDetectionsMetadataResponse(
                Limit: result.RecentDetections.Metadata.Limit,
                WorkspaceId: result.RecentDetections.Metadata.WorkspaceId,
                Timestamp: result.RecentDetections.Metadata.Timestamp,
                Filters: result.RecentDetections.Metadata.Filters is null ? null
                    : new AnprRecentDetectionsFiltersResponse(
                        ZoneId: result.RecentDetections.Metadata.Filters.ZoneId,
                        ProjectId: result.RecentDetections.Metadata.Filters.ProjectId,
                        HardwareId: result.RecentDetections.Metadata.Filters.HardwareId,
                        MinConfidence: result.RecentDetections.Metadata.Filters.MinConfidence,
                        AccessPointId: result.RecentDetections.Metadata.Filters.AccessPointId,
                        ProjectAreaId: result.RecentDetections.Metadata.Filters.ProjectAreaId)),
            Data: result.RecentDetections.Data is null ? [] :
                [.. result.RecentDetections.Data.Select(d => new RecentDetectionResponse(
                    DetectionId:     d.DetectionId,
                    DetectionTime:   d.DetectionTime,
                    PlateCode:       d.PlateCode,
                    PlateNumber:     d.PlateNumber,
                    FullPlate:       d.FullPlate,
                    ConfidenceScore: d.ConfidenceScore,
                    VehicleColor:    d.VehicleColor,
                    VehicleType:     d.VehicleType,
                    VehicleMake:     d.VehicleMake,
                    VehicleModel:    d.VehicleModel,
                    Direction:       d.Direction,
                    LaneNumber:      d.LaneNumber,
                    IsValidated:     d.IsValidated,
                    IsBlacklisted:   d.IsBlacklisted,
                    CreatedAt:       d.CreatedAt,
                    ImageUrl:        d.ImageUrl,
                    PlateCropUrl:    d.PlateCropUrl,
                    Latitude:        d.Latitude,
                    Longitude:       d.Longitude,
                    AccessPoint: d.AccessPoint is null ? null : new RecentDetectionAccessPointResponse(
                        Id:        d.AccessPoint.Id,
                        Prefix:    d.AccessPoint.Prefix,
                        Type:      d.AccessPoint.Type,
                        Latitude:  d.AccessPoint.Latitude,
                        Longitude: d.AccessPoint.Longitude,
                        Name:      d.AccessPoint.Name),
                    Hardware: d.Hardware is null ? null : new RecentDetectionHardwareResponse(
                        Id:              d.Hardware.Id,
                        Manufacturer:    d.Hardware.Manufacturer,
                        Model:           d.Hardware.Model,
                        SerialNumber:    d.Hardware.SerialNumber,
                        Status:          d.Hardware.Status,
                        HardwareType:    d.Hardware.HardwareType,
                        FirmwareVersion: d.Hardware.FirmwareVersion),
                    Project: d.Project is null ? null : new RecentDetectionProjectResponse(
                        Id:   d.Project.Id,
                        Name: d.Project.Name),
                    Zone: d.Zone is null ? null : new RecentDetectionZoneResponse(
                        Id:   d.Zone.Id,
                        Code: d.Zone.Code,
                        Name: d.Zone.Name),
                    Area: d.Area is null ? null : new RecentDetectionAreaResponse(
                        Id:   d.Area.Id,
                        Name: d.Area.Name),
                    Country: d.Country is null ? null : new RecentDetectionCountryResponse(
                        Id:   d.Country.Id,
                        Code: d.Country.Code,
                        Name: d.Country.Name),
                    State: d.State is null ? null : new RecentDetectionStateResponse(
                        Id:   d.State.Id,
                        Name: d.State.Name),
                    Category: d.Category is null ? null : new RecentDetectionCategoryResponse(
                        Id:   d.Category.Id,
                        Name: d.Category.Name)))])
    );
        }
    }
}
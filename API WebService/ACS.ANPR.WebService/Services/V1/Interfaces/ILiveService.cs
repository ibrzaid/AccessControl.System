using ACS.Models.Response.V1.ANPRService.Live;

namespace ACS.ANPR.WebService.Services.V1.Interfaces
{
    public interface ILiveService
    {
        Task<LiveResponse> GetMapViewAsync(
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
           CancellationToken cancellationToken = default);
    }
}


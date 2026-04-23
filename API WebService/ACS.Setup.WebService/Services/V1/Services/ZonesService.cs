using ACS.License.V1;
using System.Text.Json;
using System.Globalization;
using ACS.Service.V1.Interfaces;
using ACS.Models.Request.V1.SetupService.Zone;
using ACS.Database.IDataAccess.SetupService.V1;
using ACS.Models.Response.V1.SetupService.Zone;
using ACS.BusinessEntities.SetupService.V1.Zone;
using ACS.Setup.WebService.Services.V1.Interfaces;

namespace ACS.Setup.WebService.Services.V1.Services
{
    public class ZonesService(ILicenseManager licenseManager) : Service.Service(licenseManager), IZonesService
    {
        private IZoneDataAccess this[Connection conn] => conn.Type switch
        {
            Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.SetupService.V1.ZoneDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
            _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in ZonesService.")
        };
        private static string? ToJson(object? obj) => obj is null ? null : JsonSerializer.Serialize(obj);
        private static decimal ParseCoord(string? s) => decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;

        public async Task<GetZonesResponse> GetZonesAsync(int workspaceId, int callerId, int projectAreaId, int limit, int offset, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetZonesAsync(workspaceId, callerId, projectAreaId, limit, offset, ct);
            return new GetZonesResponse
            {
                Success   = db.Success,
                Error     = db.Message,
                ErrorCode = db.ErrorCode,
                Total     = db.Total,
                CanCreate = db.CanCreate,
                Data      = [.. db.Data.Select(z => new ZoneItemResponse
                {
                    ZoneId        = z.ZoneId,
                    ProjectAreaId = z.ProjectAreaId,
                    ZoneCode      = z.ZoneCode,
                    ZoneNames     = z.ZoneNames,
                    ParentZoneId  = z.ParentZoneId,
                    Depth         = z.Depth,
                    TotalSpots    = z.TotalSpots,
                    AvailableSpots= z.AvailableSpots,
                    GracePeriod   = z.GracePeriod,
                    CenterLat     = z.CenterLat,
                    CenterLon     = z.CenterLon,
                    HasChildren   = z.HasChildren,
                    IsActive      = z.IsActive,
                    CanWrite      = z.CanWrite,
                    CreatedAt     = z.CreatedAt,
                    UpdatedAt     = z.UpdatedAt,
                    FloorNumber   = z.FloorNumber,
                    AccessLevelCode= z.AccessLevelCode,
                    AccessLevelId= z.AccessLevelId,
                    AccessLevelNames= z.AccessLevelNames,
                    Polygon= z.Polygon,
                    ZoneTypeCode= z.ZoneTypeCode,
                    ZoneTypeId= z.ZoneTypeId,
                    ZoneTypeNames= z.ZoneTypeNames,
                })]
            };
        }
        public async Task<OperationZoneResultResponse> CreateZoneAsync(int workspaceId, int callerId, CreateZoneRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].CreateZoneAsync(
                workspaceId, callerId, req.ProjectAreaId,
                ToJson(req.ZoneNames)!, req.ZoneCode, req.ZoneTypeId, req.ParentZoneId,
                req.TotalSpots, req.GracePeriod, req.AccessLevelId,
                req.CenterLatitude, req.CenterLongitude,
                ToJson(req.PolygonCoordinates), req.FloorNumber,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }

        public async Task<OperationZoneResultResponse> UpdateZoneAsync(int workspaceId, int callerId, int zoneId, UpdateZoneRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].UpdateZoneAsync(
                workspaceId, callerId, zoneId,
                req.ZoneNames is null ? null : ToJson(req.ZoneNames),
                req.ZoneCode, req.TotalSpots, req.GracePeriod, req.CenterLatitude, req.CenterLongitude,
                ToJson(req.PolygonCoordinates), req.IsActive, req.FloorNumber,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }

        public async Task<OperationZoneResultResponse> DeleteZoneAsync(int workspaceId, int callerId, int zoneId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            if (!decimal.TryParse(latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _latitude)) _latitude = 0;
            if (!decimal.TryParse(longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _longitude)) _longitude = 0;
            var db = await this[license?.DB!].DeleteZoneAsync(workspaceId, callerId, zoneId, ip, ua, null, requestId, _latitude, _longitude, ct);
            return MapWrite(db);
        }


        private static OperationZoneResultResponse MapWrite(OperationResultEntity e) => new()
        {
            Success       = e.Success,
            Error         = e.Message,
            ErrorCode     = e.ErrorCode,
            ProjectId     = e.ProjectId,
            ProjectAreaId = e.ProjectAreaId,
            ZoneId        = e.ZoneId,
            AccessPointId = e.AccessPointId,
            HardwareId    = e.HardwareId,
        };
    }
}

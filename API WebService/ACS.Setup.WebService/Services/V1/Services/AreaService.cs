using ACS.License.V1;
using System.Text.Json;
using System.Globalization;
using ACS.Service.V1.Interfaces;
using ACS.Models.Request.V1.SetupService.Area;
using ACS.Models.Response.V1.SetupService.Area; 
using ACS.Database.IDataAccess.SetupService.V1;
using ACS.BusinessEntities.SetupService.V1.Area;
using ACS.Setup.WebService.Services.V1.Interfaces;

namespace ACS.Setup.WebService.Services.V1.Services
{
    public class AreaService(ILicenseManager licenseManager) : Service.Service(licenseManager), IAreaService
    {
        private IAreaDataAccess this[Connection conn] => conn.Type switch
        {
            Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.SetupService.V1.AreaDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
            _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in AreaServicee.")
        };
        private static string? ToJson(object? obj) => obj is null ? null : JsonSerializer.Serialize(obj);
        private static decimal ParseCoord(string? s) => decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;

        public async Task<GetAreasResponse> GetAreasAsync(int workspaceId, int callerId, int projectId, int limit, int offset, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetAreasAsync(workspaceId, callerId, projectId, limit, offset, ct);
            return new GetAreasResponse
            {
                Success   = db.Success,
                Error     = db.Message,
                ErrorCode = db.ErrorCode,
                Total     = db.Total,
                CanCreate = db.CanCreate,
                Data      = [.. db.Data.Select(a => new AreaItemResponse
                {
                    ProjectAreaId  = a.ProjectAreaId,
                    ProjectId      = a.ProjectId,
                    AreaNames      = a.AreaNames,
                    Description    = a.Description,
                    TotalSpots     = a.TotalSpots,
                    AvailableSpots = a.AvailableSpots,
                    CenterLatitude      = a.CenterLatitude,
                    CenterLongitude      = a.CenterLongitude,
                    IsActive       = a.IsActive,
                    CanWrite       = a.CanWrite,
                    CreatedAt      = a.CreatedAt,
                    UpdatedAt      = a.UpdatedAt,
                    AreaTypeCode  = a.AreaTypeCode,
                    AreaTypeId= a.AreaTypeId,
                    AreaTypeNames= a.AreaTypeNames,
                    FloorNumber= a.FloorNumber,
                    PolygonCoords= a.PolygonCoords                    
                })]
            };
        }

        public async Task<OperationAreaResultResponse> CreateAreaAsync(int workspaceId, int callerId, CreateAreaRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].CreateAreaAsync(
                workspaceId, callerId, req.ProjectId,
                ToJson(req.AreaNames)!, req.AreaTypeId, req.Description,
                req.TotalSpots, req.CenterLatitude, req.CenterLongitude,
                ToJson(req.PolygonCoordinates), req.FloorNumber,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }

        public async Task<OperationAreaResultResponse> UpdateAreaAsync(int workspaceId, int callerId, int projectAreaId, UpdateAreaRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].UpdateAreaAsync(
                workspaceId, callerId, projectAreaId,
                req.AreaNames is null ? null : ToJson(req.AreaNames),
                req.Description, req.TotalSpots, req.CenterLatitude, req.CenterLongitude,
                ToJson(req.PolygonCoordinates), req.IsActive, req.FloorNumber,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }

        public async Task<OperationAreaResultResponse> DeleteAreaAsync(int workspaceId, int callerId, int projectAreaId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            if (!decimal.TryParse(latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _latitude)) _latitude = 0;
            if (!decimal.TryParse(longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _longitude)) _longitude = 0;
            var db = await this[license?.DB!].DeleteAreaAsync(workspaceId, callerId, projectAreaId, ip, ua, null, requestId, _latitude, _longitude, ct);
            return MapWrite(db);
        }


        private static OperationAreaResultResponse MapWrite(OperationResultEntity e) => new()
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

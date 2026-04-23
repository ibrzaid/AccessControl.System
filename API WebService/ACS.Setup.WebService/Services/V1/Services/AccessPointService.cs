using ACS.License.V1;
using System.Globalization;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.SetupService.V1;
using ACS.Setup.WebService.Services.V1.Interfaces;
using ACS.Models.Request.V1.SetupService.AccessPoint;
using ACS.Models.Response.V1.SetupService.AccessPoint;
using ACS.BusinessEntities.SetupService.V1.AccessPoint;

namespace ACS.Setup.WebService.Services.V1.Services
{
    public class AccessPointService(ILicenseManager licenseManager) : Service.Service(licenseManager), IAccessPointService
    {
        private IAccessPointDataAccess this[Connection conn] => conn.Type switch
        {
            Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.SetupService.V1.AccessPointDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
            _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in AccessPointService.")
        };
        private static decimal ParseCoord(string? s) => decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;

        public async Task<GetAccessPointsResponse> GetAccessPointsAsync(int workspaceId, int callerId, int zoneId, int limit, int offset, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetAccessPointsAsync(workspaceId, callerId, zoneId, limit, offset, ct);
            return new GetAccessPointsResponse
            {
                Success   = db.Success,
                Error     = db.Message,
                ErrorCode = db.ErrorCode,
                Total     = db.Total,
                CanCreate = db.CanCreate,
                Data      = [.. db.Data.Select(ap => new AccessPointItemResponse
                {
                    AccessPointId = ap.AccessPointId,
                    ZoneId        = ap.ZoneId,
                    Name          = ap.Name,
                    Prefix        = ap.Prefix,
                    SerialNumber  = ap.SerialNumber,
                    ApTypeId      = ap.ApTypeId,
                    AccessLevelId = ap.AccessLevelId,
                    Latitude      = ap.Latitude,
                    Longitude     = ap.Longitude,
                    IsActive      = ap.IsActive,
                    CanWrite      = ap.CanWrite,
                    CreatedAt     = ap.CreatedAt,
                    UpdatedAt     = ap.UpdatedAt,
                    AccessPointTypeCode = ap.AccessPointTypeCode,
                    AccessPointTypeNames= ap.AccessPointTypeNames,
                    OrientationDegrees= ap.OrientationDegrees,
                    PositionX           = ap.PositionX,
                    PositionY = ap.PositionY
                })]
            };
        }

        public async Task<OperationAccessPointResultResponse> CreateAccessPointAsync(int workspaceId, int callerId, CreateAccessPointRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].CreateAccessPointAsync(
                workspaceId, callerId, req.ZoneId,
                req.AccessPointName, req.Prefix, req.SerialNumber,
                req.ApTypeId,  req.ApLatitude, req.ApLongitude,
                req.PositionY, req.PositionY, req.OrientationDegrees,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }

        public async Task<OperationAccessPointResultResponse> UpdateAccessPointAsync(int workspaceId, int callerId, int accessPointId, UpdateAccessPointRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].UpdateAccessPointAsync(
                workspaceId, callerId, accessPointId,
                req.AccessPointName, req.Prefix, req.SerialNumber,
                req.ApTypeId, req.ApLatitude, req.ApLongitude,
                req.PositionY, req.PositionY, req.OrientationDegrees,
                req.IsActive,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }

        public async Task<OperationAccessPointResultResponse> DeleteAccessPointAsync(int workspaceId, int callerId, int accessPointId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].DeleteAccessPointAsync(workspaceId, callerId, accessPointId, ip, ua, null, requestId, ParseCoord(latitude), ParseCoord(longitude), ct);
            return MapWrite(db);
        }


        private static OperationAccessPointResultResponse MapWrite(OperationResultEntity e) => new()
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

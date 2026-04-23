using ACS.License.V1;
using System.Globalization;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.SetupService.V1;
using ACS.Models.Request.V1.SetupService.Hardware;
using ACS.Setup.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.SetupService.Hardware;
using ACS.BusinessEntities.SetupService.V1.Hardware;

namespace ACS.Setup.WebService.Services.V1.Services
{
    public class HardwareService(ILicenseManager licenseManager) : Service.Service(licenseManager), IHardwareService
    {
        private IHardwareDataAccess this[Connection conn] => conn.Type switch
        {
            Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.SetupService.V1.HardwareDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
            _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in HardwareService.")
        };
        private static decimal ParseCoord(string? s) => decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;

        public async Task<GetHardwareResponse> GetHardwareAsync(int workspaceId, int callerId, int accessPointId, int limit, int offset, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetHardwareAsync(workspaceId, callerId, accessPointId, limit, offset, ct);
            return new GetHardwareResponse
            {
                Success   = db.Success,
                Error     = db.Message,
                ErrorCode = db.ErrorCode,
                Total     = db.Total,
                CanCreate = db.CanCreate,
                Data      = [.. db.Data.Select(h => new HardwareItemResponse
                {
                    HardwareId              = h.HardwareId,
                    AccessPointId           = h.AccessPointId,
                    HardwareTypeCode        = h.HardwareTypeCode,
                    Manufacturer            = h.Manufacturer,
                    Model                   = h.Model,
                    SerialNumber            = h.SerialNumber,
                    FirmwareVersion         = h.FirmwareVersion,
                    StatusCode              = h.StatusCode,
                    IsActive                = h.IsActive,
                    CanWrite                = h.CanWrite,
                    LastActivity            = h.LastActivity,
                    DetectionCount          = h.DetectionCount,
                    CreatedAt               = h.CreatedAt,
                    UpdatedAt               = h.UpdatedAt,
                    HardwareConfiguration   = h.HardwareConfiguration,
                    HardwareStatusId        = h.HardwareStatusId,
                    HardwareStatusNames     = h.HardwareStatusNames,
                    HardwareTypeId          = h.HardwareTypeId,
                    HardwareTypeNames       = h.HardwareTypeNames,
                    LastMaintenanceDate     = h.LastMaintenanceDate,
                    NextMaintenanceDate     = h.NextMaintenanceDate,
                    ParentHardwareId        = h.ParentHardwareId
                })]
            };
        }

        public async Task<OperationHardwareResultResponse> CreateHardwareAsync(int workspaceId, int callerId, CreateHardwareRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].CreateHardwareAsync(
                workspaceId, callerId, req.AccessPointId,
                req.HardwareTypeCode, req.Manufacturer, req.Model,
                req.SerialNumber, req.FirmwareVersion, req.HardwareStatusCode, 
                req.LastMaintenanceDate, req.NextMaintenanceDate, req.HardwareConfiguration,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }

        public async Task<OperationHardwareResultResponse> UpdateHardwareAsync(int workspaceId, int callerId, int hardwareId, UpdateHardwareRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].UpdateHardwareAsync(
                workspaceId, callerId, hardwareId,
                req.HardwareTypeCode, req.Manufacturer, req.Model,
                req.SerialNumber, req.FirmwareVersion, req.HardwareStatusCode,
                req.LastMaintenanceDate, req.NextMaintenanceDate, req.HardwareConfiguration,
                req.IsActive,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }

        public async Task<OperationHardwareResultResponse> DeleteHardwareAsync(int workspaceId, int callerId, int hardwareId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].DeleteHardwareAsync(workspaceId, callerId, hardwareId, ip, ua, null, requestId, 
                ParseCoord(latitude), ParseCoord(longitude), ct);
            return MapWrite(db);
        }


        private static OperationHardwareResultResponse MapWrite(OperationResultEntity e) => new()
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

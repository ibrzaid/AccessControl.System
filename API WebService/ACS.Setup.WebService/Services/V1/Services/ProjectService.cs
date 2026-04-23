using ACS.License.V1;
using System.Text.Json;
using System.Globalization;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.SetupService.V1;
using ACS.Models.Request.V1.SetupService.Project;
using ACS.Models.Response.V1.SetupService.Project;
using ACS.Setup.WebService.Services.V1.Interfaces;
using ACS.BusinessEntities.SetupService.V1.Project;

namespace ACS.Setup.WebService.Services.V1.Services
{
    public class ProjectService(ILicenseManager licenseManager) : Service.Service(licenseManager), IProjectService
    {
        private IProjectDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.SetupService.V1.ProjectDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in ProjectService.")
                };
            }
        }
        private static string? ToJson(object? obj) => obj is null ? null : JsonSerializer.Serialize(obj);
        private static decimal ParseCoord(string? s) =>
            decimal.TryParse(s, NumberStyles.Any,CultureInfo.InvariantCulture, out var v) ? v : 0m;

        public async Task<GetProjectsResponse> GetProjectsAsync(int workspaceId, int callerId, int limit, int offset, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetProjectsAsync(workspaceId, callerId, limit, offset, ct);
            return new GetProjectsResponse
            {
                Success          = db.Success,
                Error            = db.Message,
                ErrorCode        = db.ErrorCode,
                Total            = db.Total,
                CanCreateProject = db.CanCreateProject,
                Data             = [.. db.Data.Select(p => new ProjectItemResponse
                {
                    ProjectId           = p.ProjectId,
                    Names               = p.ProjectNames,
                    Description         = p.Description,
                    Address             = p.Address,
                    City                = p.City,
                    State               = p.State,
                    CountryId           = p.CountryId,
                    PostalCode          = p.PostalCode,
                    Latitude            = p.Latitude,
                    Longitude           = p.Longitude,
                    Timezone            = p.Timezone,
                    IsPublic            = p.IsPublic,
                    IsActive            = p.IsActive,
                    CanWrite            = p.CanWrite,
                    CreatedAt           = p.CreatedAt,
                    UpdatedAt           = p.UpdatedAt,
                    CountryNames        = p.CountryNames,
                    ProjectTypeCode     = p.ProjectTypeCode,
                    ProjectTypeId       = p.ProjectTypeId,
                    ProjectTypeNames    = p.ProjectTypeNames
                    
                })]
            };
        }

        public async Task<OperationProjectResultResponse> CreateProjectAsync(int workspaceId, int callerId, CreateProjectRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].CreateProjectAsync(
                workspaceId, callerId,
                ToJson(req.ProjectNames)!, req.Description, req.ProjectType,
                req.Address, req.City, req.State, req.CountryId, req.PostalCode,
                req.ProjectLatitude, req.ProjectLongitude, req.Timezone, req.IsPublic,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }

        public async Task<OperationProjectResultResponse> DeleteProjectAsync(int workspaceId, int callerId, int projectId, string? ip, string? ua, string? requestId, string latitude, string longitude, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            if (!decimal.TryParse(latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _latitude)) _latitude = 0;
            if (!decimal.TryParse(longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal _longitude)) _longitude = 0;
            var db = await this[license?.DB!].DeleteProjectAsync(workspaceId, callerId, projectId, ip, ua, null, requestId, _latitude, _longitude, ct);
            return MapWrite(db);
        }

       

        public async Task<OperationProjectResultResponse> UpdateProjectAsync(int workspaceId, int callerId, int projectId, UpdateProjectRequest req, string? ip, string? ua, string? deviceInfo, string? requestId, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].UpdateProjectAsync(
                workspaceId, callerId, projectId,
                req.ProjectNames is null ? null : ToJson(req.ProjectNames),
                req.Description, req.ProjectType, req.Address, req.City, req.State, req.CountryId, req.PostalCode,
                req.ProjectLatitude, req.ProjectLongitude, req.Timezone, req.IsPublic, req.IsActive,
                ip, ua, deviceInfo, requestId,
                ParseCoord(req.Latitude), ParseCoord(req.Longitude), ct);
            return MapWrite(db);
        }


        private static OperationProjectResultResponse MapWrite(OperationResultEntity e) => new()
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

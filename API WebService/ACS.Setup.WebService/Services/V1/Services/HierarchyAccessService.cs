using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.SetupService.V1;
using ACS.Setup.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.SetupService.HierarchyAccess;
using ACS.BusinessEntities.SetupService.V1.HierarchyAccess;

namespace ACS.Setup.WebService.Services.V1.Services
{
    public class HierarchyAccessService(ILicenseManager licenseManager) : Service.Service(licenseManager), IHierarchyAccessService
    {
        private IHierarchyAccessDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.SetupService.V1.HierarchyAccessDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in HierarchyAccessService.")
                };
            }
        }

        /// <summary>
        /// Returns the hierarchy tree for the calling user starting at
        /// their scope level. result_level in the response tells the
        /// caller what the top-level data items are.
        /// </summary>
        /// <param name="callerUserId"></param>
        /// <param name="callerWorkspaceId"></param>
        /// <param name="projectId"></param>
        /// <param name="projectAreaId"></param>
        /// <param name="zoneId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HierarchyAccessResponse> GetHierarchyAccessAsync(int callerUserId, int callerWorkspaceId, int? projectId = null, int? projectAreaId = null, int? zoneId = null, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetHierarchyTreeAsync(callerUserId, callerWorkspaceId, projectId, projectAreaId, zoneId, cancellationToken);
            if (!db.Success)
                return new HierarchyAccessResponse
                {
                    Success     = false,
                    Error     = db.Message,
                    ErrorCode   = db.ErrorCode,
                    ResultLevel = string.Empty,
                    Total       = 0,
                    ID =  db.ID,
                    Names = db.Names,
                    Data        = new List<object>()
                };

            return db.ResultLevel switch
            {
                "workspace" => new HierarchyAccessResponse
                {
                    Success     = true,
                    ResultLevel = db.ResultLevel,
                    Total       = db.Total,
                    ID = db.ID,
                    Names= db.Names,
                    Data        = db.Projects.Select(MapProject).ToList()
                },
                "project" => new HierarchyAccessResponse
                {
                    Success     = true,
                    ResultLevel = db.ResultLevel,
                    Total       = db.Total,
                    ID = db.ID,
                    Names= db.Names,
                    Data        = db.ProjectAreas.Select(MapArea).ToList()
                },
                "project_area" => new HierarchyAccessResponse
                {
                    Success     = true,
                    ResultLevel = db.ResultLevel,
                    Total       = db.Total,
                    ID = db.ID,
                    Names= db.Names,
                    Data        = db.Zones.Select(MapZone).ToList()
                },
                "zone" => new HierarchyAccessResponse
                {
                    Success     = true,
                    ResultLevel = db.ResultLevel,
                    Total       = db.Total,
                    ID = db.ID,
                    Names= db.Names,
                    Data        = db.AccessPoints.Select(MapAccessPoint).ToList()
                },
                "access_point" => new HierarchyAccessResponse
                {
                    Success     = true,
                    ResultLevel = db.ResultLevel,
                    Total       = db.Total,
                    ID = db.ID,
                    Names= db.Names,
                    Data        = db.Hardware.Select(MapHardware).ToList()
                },
                _ => new HierarchyAccessResponse
                {
                    Success     = false,
                    Error     = $"Unknown result_level: {db.ResultLevel}",
                    ErrorCode   = "UNKNOWN_RESULT_LEVEL",
                    ResultLevel = db.ResultLevel,
                    Total       = 0,
                    Data        = new List<object>()
                }
            };
        }

        private static HierarchyProjectResponse MapProject(HierarchyProjectEntity e) => new()
        {
            ProjectId          = e.ProjectId,
            ProjectNames       = e.ProjectNames,
            ProjectDescription = e.ProjectDescription,
            ProjectLatitude    = e.ProjectLatitude,
            ProjectLongitude   = e.ProjectLongitude,
            ProjectAreas       = [.. e.ProjectAreas.Select(MapArea)]
        };

        private static HierarchyProjectAreaResponse MapArea(HierarchyProjectAreaEntity e) => new()
        {
            ProjectAreaId      = e.ProjectAreaId,
            ProjectAreaNames   = e.ProjectAreaNames,
            TotalSpots         = e.TotalSpots,
            AvailableSpots     = e.AvailableSpots,
            CenterLatitude     = e.CenterLatitude,
            CenterLongitude    = e.CenterLongitude,
            PolygonCoordinates = e.PolygonCoordinates,
            Zones              = [.. e.Zones.Select(MapZone)]
        };

        private static HierarchyZoneResponse MapZone(HierarchyZoneEntity e) => new()
        {
            ZoneId             = e.ZoneId,
            ZoneCode           = e.ZoneCode,
            ZoneNames          = e.ZoneNames,
            ZoneTypeCode       = e.ZoneTypeCode,
            ParentZoneId       = e.ParentZoneId,
            Depth              = e.Depth,
            IsActive           = e.IsActive,
            TotalSpots         = e.TotalSpots,
            AvailableSpots     = e.AvailableSpots,
            GracePeriod        = e.GracePeriod,
            CenterLatitude     = e.CenterLatitude,
            CenterLongitude    = e.CenterLongitude,
            PolygonCoordinates = e.PolygonCoordinates,
            HasChildren        = e.HasChildren,
            Children           = [.. e.Children.Select(MapZone)], 
            AccessPoints       = [.. e.AccessPoints.Select(MapAccessPoint)]
        };


        private static HierarchyAccessPointResponse MapAccessPoint(HierarchyAccessPointEntity e) => new()
        {
            AccessPointId        = e.AccessPointId,
            AccessPointName      = e.AccessPointName,
            Prefix               = e.Prefix,
            SerialNumber         = e.SerialNumber,
            AccessPointTypeCode  = e.AccessPointTypeCode,
            AccessPointTypeNames = e.AccessPointTypeNames,
            AccessPointLatitude  = e.AccessPointLatitude,
            AccessPointLongitude = e.AccessPointLongitude,
            ZoneId               = e.ZoneId,
            IsActive             = e.IsActive,
            Hardware             = [.. e.Hardware.Select(MapHardware)]
        };

        private static HierarchyHardwareResponse MapHardware(HierarchyHardwareEntity e) => new()
        {
            HardwareId          = e.HardwareId,
            HardwareTypeCode    = e.HardwareTypeCode,
            HardwareTypeNames   = e.HardwareTypeNames,
            Manufacturer        = e.Manufacturer,
            Model               = e.Model,
            SerialNumber        = e.SerialNumber,
            FirmwareVersion     = e.FirmwareVersion,
            HardwareStatusCode  = e.HardwareStatusCode,
            HardwareStatusNames = e.HardwareStatusNames,
            AccessPointId       = e.AccessPointId,
            IsActive            = e.IsActive,
            LastActivity        = e.LastActivity,
            DetectionCount      = e.DetectionCount
        };

    }
}

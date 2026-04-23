using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using ACS.Database.IDataAccess.MasterService.V1;
using ACS.Master.WebService.Services.V1.Interfaces;
using ACS.Models.Response.V1.MasterService.SetupReference;

namespace ACS.Master.WebService.Services.V1.Services
{
    public class SetupReferenceService(ILicenseManager licenseManager) : Service.Service(licenseManager), ISetupReferenceService
    {
        private ISetupReferenceDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.MasterService.V1.SetupReferenceDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in SetupReferenceService.")
                };
            }
        }

        public async Task<GetSetupReferenceResponse> GetAccessLevelsAsync(CancellationToken ct = default) => await GetRef("access_level", ct);

        public async Task<GetSetupReferenceResponse> GetAccessPointTypesAsync(CancellationToken ct = default) => await GetRef("access_point_type", ct);

        public async Task<GetSetupReferenceResponse> GetAreaTypesAsync(CancellationToken ct = default) => await GetRef("area_type", ct);

        public async Task<GetSetupReferenceResponse> GetProjectTypesAsync(CancellationToken ct = default) => await GetRef("project_type", ct);

        public async Task<GetSetupReferenceResponse> GetStatusesAsync(CancellationToken ct = default) => await GetRef("status", ct);

        public async Task<GetSetupReferenceResponse> GetZoneTypesAsync(CancellationToken ct = default) => await GetRef("zone_type", ct);


        public async Task<GetSetupReferenceResponse> GetHardwareTypesAsync(CancellationToken ct = default) => await GetRef("hardware_type", ct);

        public async Task<GetSetupReferenceResponse> GetHardwareStatusesAsync(CancellationToken ct = default) => await GetRef("hardware_status", ct);

        public async Task<GetSetupReferenceResponse> GetCountriesAsync(CancellationToken ct = default) => await GetRef("counties", ct);





        private async Task<GetSetupReferenceResponse> GetRef(string refType, CancellationToken ct = default)
        {
            var license = this.LicenseManager.GetLicense();
            var db = await this[license?.DB!].GetReferenceAsync(refType, ct);
            return new GetSetupReferenceResponse
            {
                Success = db.Success,
                Error   = db.Message,
                ErrorCode = db.ErrorCode,
                Data    = [.. db.Data.Select(r => new SetupReferenceItemResponse
                {
                    Id    = r.Id,
                    Code  = r.Code,
                    Names = r.Names,
                    Config= r.Config?.ToDictionary(
                        c => c.Key,
                        c => new FieldConfigResponse
                        {
                           Exp = c.Value.Exp,
                           Text = c.Value.Text,
                           Required = c.Value.Required,
                           FieldName= c.Value.FieldName,
                        })
                })]
            };
        }

    }
}

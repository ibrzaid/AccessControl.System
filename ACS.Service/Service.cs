using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using ACS.Service.V1.Interfaces;


namespace ACS.Service
{
    /// <summary>
    ///  Base Service
    /// </summary>
    /// <param name="connections"></param>
    public class Service(ILicenseManager licenseManager)
    {
        protected ILicenseManager LicenseManager { get; private set; } = licenseManager;

        public readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
}

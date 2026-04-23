using ACS.License.V1;

namespace ACS.Service.V1.Interfaces
{
    public interface ILicenseManager
    {
        bool IsValid();
        LicenseStatus GetStatus();
        string GetLicensePath();
        License.V1.License? GetLicense();
    }
}

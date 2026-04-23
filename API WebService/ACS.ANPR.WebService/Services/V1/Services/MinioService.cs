using Minio;
using Minio.DataModel.Args;
using ACS.Service.V1.Interfaces;
using ACS.ANPR.WebService.Services.V1.Interfaces;

namespace ACS.ANPR.WebService.Services.V1.Services
{
    public class MinioService : Service.Service, IMinioService
    {
        private readonly IMinioClient _client;
        public MinioService(ILicenseManager licenseManager) : base(licenseManager)
        {
            var license = this.LicenseManager.GetLicense();

            _client = new MinioClient()
            .WithEndpoint(license?.Minio?.Url)
            .WithCredentials(license?.Minio?.User, license?.Minio?.Password)
            .Build();
        }

        public async Task<string> UploadImageAsync(Stream stream, string workspace, string project, string area, string zone,  string accesspoint, string hardware, string fileName, string contentType)
        {
            var license = this.LicenseManager.GetLicense();
            var objectName = $"anpr/{workspace}/{project}/{area}/{zone}/{accesspoint}/{hardware}/{DateTime.UtcNow:yyyy/MM/dd}/{fileName}";
            await _client.PutObjectAsync(new PutObjectArgs()
                .WithBucket(license?.Minio?.Name)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType));
            return objectName;
        }

        public async Task<string> GetImageUrlAsync(string objectName)
        {
            var license = this.LicenseManager.GetLicense();
            return await _client.PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket(license?.Minio?.Name)
                .WithObject(objectName)
                .WithExpiry(60 * 5));
        }
    }
}

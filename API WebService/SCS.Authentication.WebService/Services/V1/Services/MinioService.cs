using ACS.Authentication.WebService.Services.V1.Interfaces;
using ACS.Service.V1.Interfaces;
using Minio;
using Minio.DataModel.Args;
using Pipelines.Sockets.Unofficial.Arenas;

namespace ACS.Authentication.WebService.Services.V1.Services
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

        public async Task<string> UploadImageAsync(Stream stream, string workspace, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            var objectName = $"avatar/{workspace}/{fileName}";
            await _client.PutObjectAsync(new PutObjectArgs()
                .WithBucket("avatars")
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType), cancellationToken);
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

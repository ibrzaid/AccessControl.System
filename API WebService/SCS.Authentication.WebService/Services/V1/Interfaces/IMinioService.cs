namespace ACS.Authentication.WebService.Services.V1.Interfaces
{
    public interface IMinioService
    {
        Task<string> UploadImageAsync(Stream stream, string workspace,  string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<string> GetImageUrlAsync(string objectName);
    }
}

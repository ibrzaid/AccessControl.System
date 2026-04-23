namespace ACS.ANPR.WebService.Services.V1.Interfaces
{
    public interface IMinioService
    {
        Task<string> UploadImageAsync(Stream stream, string workspace, string project, string area,string zone, string accesspoint, string hardware, string fileName, string contentType);
        Task<string> GetImageUrlAsync(string objectName);
    }
}

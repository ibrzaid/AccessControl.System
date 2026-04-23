using ACS.Background;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using ACS.ANPR.WebService.Models.V1.Hikvision;
using ACS.ANPR.WebService.Services.V1.Interfaces;

namespace ACS.ANPR.WebService.Services.V1.Services
{
    public class HikVisionService(IBackgroundTaskQueue backgroundTaskQueue) : IHikVisionService
    {
        

        public async Task<ContentResult> ProcessAsync(string id,  IFormCollection collection, CancellationToken cancellationToken = default)
        {
            var xmlFile = collection.Files.FirstOrDefault(f => f.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) || f.ContentType == "application/xml");
            if (xmlFile == null)  return GenerateErrorResponse("No XML data found");

            using var reader = new StreamReader(xmlFile.OpenReadStream());
            var xmlContent = await reader.ReadToEndAsync(cancellationToken);

            EventNotificationAlert? eventAlert = null;
            var serializer = new XmlSerializer(typeof(EventNotificationAlert));
            using var xmlReader = new StringReader(xmlContent);
            eventAlert = serializer.Deserialize(xmlReader) as EventNotificationAlert;
            if(eventAlert == null )  return GenerateErrorResponse("Failed to deserialize XML data");
            if (eventAlert.EventType == "heartBeat")
            {
                return GenerateSuccessResponse();
            }

            var dateDir = DateTime.Now.ToString("yyyy-MM-dd");
            var baseDir = Path.Combine("Images", dateDir);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");


            var imageFiles = await Task.WhenAll(
                collection.Files.Where(f => f.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                .Select(async f =>
                {
                    var extension = Path.GetExtension(f.FileName);

                    string type = f.Name.Contains("detectionPicture", StringComparison.OrdinalIgnoreCase)
                        ? "detection"
                        : f.Name.Contains("licensePlatePicture", StringComparison.OrdinalIgnoreCase)
                        ? "licenseplate"
                        : "other";

                    string fileName = type switch
                    {
                        "detection" => $"{f.Name}_{timestamp}_vehicle{extension}",
                        "licenseplate" => $"{f.Name}_{timestamp}_plate{extension}",
                        _ => $"{f.Name}_{timestamp}_{f.FileName}"
                    };

                    using var ms = new MemoryStream();
                    await f.CopyToAsync(ms, cancellationToken);
                    return (Content: ms.ToArray(), FileName: Path.Combine(baseDir, fileName), ContentType: f.ContentType, Type: type);
                }));

            

            backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                Directory.CreateDirectory(baseDir);
                foreach (var image in imageFiles)
                {
                    await File.WriteAllBytesAsync(image.FileName, image.Content, token);
                }
            });



            return GenerateSuccessResponse();

        }

        public ContentResult GenerateSuccessResponse() => new()
        {
            Content =  @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Response>
    <result>OK</result>
    <statusCode>200</statusCode>
    <statusString>Success</statusString>
</Response>",
            ContentType = MediaTypeHeaderValue.Parse("application/xml").ToString(),
        };


        public ContentResult GenerateErrorResponse(string errorMessage) => new ()
        {
            Content =  $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Response>
    <result>ERROR</result>
    <statusCode>500</statusCode>
    <statusString>{EscapeXml(errorMessage)}</statusString>
</Response>",
            ContentType = MediaTypeHeaderValue.Parse("application/xml").ToString(),
        };

        private static string EscapeXml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

    }
}

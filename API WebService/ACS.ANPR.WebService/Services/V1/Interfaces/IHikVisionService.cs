using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace ACS.ANPR.WebService.Services.V1.Interfaces
{
    public interface IHikVisionService
    {
        Task<ContentResult> ProcessAsync(string id, IFormCollection collection, CancellationToken cancellationToken = default);

        ContentResult GenerateSuccessResponse();

        ContentResult GenerateErrorResponse(string errorMessage);
    }
}

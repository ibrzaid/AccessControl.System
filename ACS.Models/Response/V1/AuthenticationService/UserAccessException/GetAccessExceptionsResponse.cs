using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserAccessException
{
    public class GetAccessExceptionsResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "Exceptions")]
        public List<AccessExceptionResponse> Data { get; set; } = [];
    }
}

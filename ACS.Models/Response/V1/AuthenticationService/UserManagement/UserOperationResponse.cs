using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class UserOperationResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "Data")]
        public object? Data { get; set; }
    }
}


using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserRole
{
    public class RoleOperationResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "Data")]
        public object? Data { get; set; }
    }
}

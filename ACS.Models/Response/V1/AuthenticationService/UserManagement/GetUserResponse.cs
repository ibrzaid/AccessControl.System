using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class GetUserResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "User")]
        public UserResponse? Data { get; set; }
    }
}

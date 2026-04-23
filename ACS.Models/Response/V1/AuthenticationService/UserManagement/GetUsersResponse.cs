using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class GetUsersResponse : BaseResponse
    {
        [JsonPropertyName("total")]
        [Display(Name = "Total")]
        public int Total { get; set; }

        [JsonPropertyName("data")]
        [Display(Name = "Users")]
        public List<UserResponse> Data { get; set; } = [];
    }
}

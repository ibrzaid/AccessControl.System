using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class GetManageableUsersResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "Manageable Users")]
        public List<ManageableUserResponse> Data { get; set; } = [];
    }
}

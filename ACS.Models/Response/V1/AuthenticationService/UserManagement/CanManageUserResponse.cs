using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class CanManageUserResponse : BaseResponse
    {
        [JsonPropertyName("can_manage")]
        [Display(Name = "Can Manage")]
        public bool CanManage { get; set; }
    }
}

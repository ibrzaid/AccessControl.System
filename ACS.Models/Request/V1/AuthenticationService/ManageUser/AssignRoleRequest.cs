using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.AuthenticationService.ManageUser
{
    public class AssignRoleRequest : BaseRequest
    {
        [JsonPropertyName("role_id")]
        [Required(ErrorMessage = "Please select a role")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid role")]
        [Display(Name = "Role")]
        public int RoleId { get; set; }
    }
}

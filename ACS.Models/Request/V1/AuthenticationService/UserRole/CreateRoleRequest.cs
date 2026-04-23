
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.AuthenticationService.UserRole
{
    public class CreateRoleRequest : BaseRequest
    {
        [JsonPropertyName("role_names")]
        [Required(ErrorMessage = "Please provide role names")]
        [Display(Name = "Role Names")]
        public Dictionary<string, string> RoleNames { get; set; } = [];

        [JsonPropertyName("role_description")]
        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        [Display(Name = "Description")]
        public string? RoleDescription { get; set; }

        [JsonPropertyName("role_permissions")]
        [Display(Name = "Permissions")]
        public List<string> RolePermissions { get; set; } = [];
    }
}

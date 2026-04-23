using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserRole
{
    public class RoleSummaryResponse
    {
        [JsonPropertyName("user_role_id")]
        [Display(Name = "Role ID")]
        public int UserRoleId { get; set; }

        [JsonPropertyName("role_names")]
        [Display(Name = "Role Name")]
        public object? RoleNames { get; set; }
    }
}

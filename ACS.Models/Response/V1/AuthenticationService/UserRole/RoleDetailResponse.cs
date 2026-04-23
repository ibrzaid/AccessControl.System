using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserRole
{
    public class RoleDetailResponse : RoleSummaryResponse
    {
        [JsonPropertyName("role_description")]
        [Display(Name = "Role Description")]
        public string? RoleDescription { get; set; }

        [JsonPropertyName("role_permissions")]
        [Display(Name = "Permissions")]
        public object? RolePermissions { get; set; }

        [JsonPropertyName("assigned_at")]
        [Display(Name = "Assigned At")]
        public DateTime AssignedAt { get; set; }

        [JsonPropertyName("assigned_by")]
        [Display(Name = "Assigned By")]
        public int? AssignedBy { get; set; }
    }
}

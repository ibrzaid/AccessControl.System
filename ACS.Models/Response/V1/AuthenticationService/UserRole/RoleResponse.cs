using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserRole
{
    public class RoleResponse
    {
        [JsonPropertyName("user_role_id")][Display(Name = "Role ID")] public int UserRoleId { get; set; }
        [JsonPropertyName("workspace_id")][Display(Name = "Workspace")] public int WorkspaceId { get; set; }
        [JsonPropertyName("role_names")][Display(Name = "Role Name")] public object? RoleNames { get; set; }
        [JsonPropertyName("role_description")][Display(Name = "Description")] public string? RoleDescription { get; set; }
        [JsonPropertyName("role_permissions")][Display(Name = "Permissions")] public object? RolePermissions { get; set; }
        [JsonPropertyName("is_active")][Display(Name = "Active")] public bool IsActive { get; set; }
        [JsonPropertyName("created_date")][Display(Name = "Created")] public DateTime CreatedDate { get; set; }
        [JsonPropertyName("updated_date")][Display(Name = "Updated")] public DateTime UpdatedDate { get; set; }
        [JsonPropertyName("user_count")][Display(Name = "Assigned Users")] public int UserCount { get; set; }
    }
}

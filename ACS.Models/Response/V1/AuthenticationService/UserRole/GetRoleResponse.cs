
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserRole
{
    public class GetRoleResponse : BaseResponse
    {
        [JsonPropertyName("data")][Display(Name = "Role")] public RoleResponse? Data { get; set; }
    }
}

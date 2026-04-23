using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserRole
{
    public class GetRolesResponse : BaseResponse
    {
        [JsonPropertyName("total")][Display(Name = "Total")] public int Total { get; set; }
        [JsonPropertyName("data")][Display(Name = "Roles")] public List<RoleResponse> Data { get; set; } = [];
    }
}

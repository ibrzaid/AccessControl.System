using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserAccess
{
    public class GrantAccessResponse :BaseResponse
    {

        [JsonPropertyName("user_access_id")]
        [Display(Name = "Access Rule ID")]
        public long UserAccessId { get; set; }

        [JsonPropertyName("action")]
        [Display(Name = "Action")]
        public string Action { get; set; } = string.Empty;
    }
}

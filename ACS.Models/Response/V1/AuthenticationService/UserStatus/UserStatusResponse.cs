using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserStatus
{
    public class UserStatusResponse
    {
        [JsonPropertyName("user_status_id")]
        [Display(Name = "Status ID")]
        public int UserStatusId { get; set; }

        [JsonPropertyName("user_status_code")]
        [Display(Name = "Status Code")]
        public string UserStatusCode { get; set; } = string.Empty;

        [JsonPropertyName("user_status_names")]
        [Display(Name = "Status Name")]
        public object? UserStatusNames { get; set; }
    }
}

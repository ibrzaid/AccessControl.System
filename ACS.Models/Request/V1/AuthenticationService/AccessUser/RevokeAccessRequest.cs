using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.AuthenticationService.AccessUser
{
    public class RevokeAccessRequest : BaseRequest
    {
        [JsonPropertyName("reason")]
        [StringLength(500, ErrorMessage = "Reason must not exceed 500 characters")]
        [Display(Name = "Reason")]
        public string? Reason { get; set; }
    }
}

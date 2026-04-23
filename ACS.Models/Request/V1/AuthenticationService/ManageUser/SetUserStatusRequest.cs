using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.AuthenticationService.ManageUser
{
    public class SetUserStatusRequest : BaseRequest
    {
        [JsonPropertyName("status_code")]
        [Required(ErrorMessage = "Please select a status")]
        [RegularExpression("^(ACTIVE|INACTIVE|SUSPENDED|LOCKED)$",
            ErrorMessage = "Status must be ACTIVE, INACTIVE, SUSPENDED, or LOCKED")]
        [Display(Name = "Status")]
        public string StatusCode { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        [StringLength(500, ErrorMessage = "Reason must not exceed 500 characters")]
        [Display(Name = "Reason")]
        public string? Reason { get; set; }
    }
}

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.AuthenticationService.ManageUser
{
    public class ResetPasswordRequest : BaseRequest
    {
        [JsonPropertyName("new_password")]
        [Required(ErrorMessage = "Please enter a new password")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        [StringLength(500, ErrorMessage = "Reason must not exceed 500 characters")]
        [Display(Name = "Reason")]
        public string? Reason { get; set; }
    }
}

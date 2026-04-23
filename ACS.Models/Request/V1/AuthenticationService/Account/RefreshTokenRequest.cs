using System.ComponentModel;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.AuthenticationService.Account
{
    public class RefreshTokenRequest : BaseRequest
    {
        /// <summary>
        /// Refresh Token to get new token
        /// </summary>
        [DefaultValue("")]
        [JsonPropertyName("refresh_token")]
        [Required(ErrorMessage = "Please enter Refresh Token")]
        [Display(Name = "Refresh Token")]
        public string? RefreshToken { get; set; }
    }
}

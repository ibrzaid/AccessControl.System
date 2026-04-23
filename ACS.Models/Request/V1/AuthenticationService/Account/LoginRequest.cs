using System.ComponentModel;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using ACS.Models.Request;

namespace ACS.Models.Request.V1.AuthenticationService.Account
{
    public class LoginRequest : BaseRequest
    {
        /// <summary>
        /// Username , Phone number Or Email Address
        /// </summary>
        [DefaultValue("ibr.zaid")]
        [JsonPropertyName("username")]
        [Required(ErrorMessage = "Please enter Username")]
        [Display(Name = "Username")]
        public string? Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [DefaultValue("admin")]
        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Please enter Password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}



using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACS.Models.Request.V1.AuthenticationService.Avatar
{
    public class UpdateAvatarRequest : BaseRequest
    {
        [JsonPropertyName("file")]
        [Display(Name = "File")]
        [Required(ErrorMessage = "File is required")]
        public IFormFile? File { get; init; }


        [DefaultValue(3)]
        [JsonPropertyName("user_id")]
        [Display(Name = "User Id")]
        [Required(ErrorMessage = "User Id is required")]
        [Range(1, int.MaxValue)]
        public int UserId { get; init; }
    }
}

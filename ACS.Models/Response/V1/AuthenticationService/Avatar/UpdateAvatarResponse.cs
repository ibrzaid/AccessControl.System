
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.Avatar
{
    public class UpdateAvatarResponse : BaseResponse
    {
        [JsonPropertyName("Path")]
        [Display(Name = "Path")]
        public string? Path {  get; set; }
    }
}

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class CreateUserResponse : BaseResponse
    {
        [JsonPropertyName("errors")]
        [Display(Name = "Errors")]
        public List<string>? Errors { get; set; }

        [JsonPropertyName("warnings")]
        [Display(Name = "Warnings")]
        public List<string>? Warnings { get; set; }

        [JsonPropertyName("data")]
        [Display(Name = "Data")]
        public CreateUserDataResponse? Data { get; set; }
    }
}

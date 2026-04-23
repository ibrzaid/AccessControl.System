using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.AccessLevel
{
    public class GetAccessLevelsResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "Access Levels")]
        public List<AccessLevelResponse> Data { get; set; } = [];
    }
}

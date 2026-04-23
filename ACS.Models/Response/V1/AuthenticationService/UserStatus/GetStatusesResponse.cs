using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserStatus
{
    public class GetStatusesResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "Statuses")]
        public List<UserStatusResponse> Data { get; set; } = [];
    }
}

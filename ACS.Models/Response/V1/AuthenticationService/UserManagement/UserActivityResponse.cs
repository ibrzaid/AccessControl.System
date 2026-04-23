using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using ACS.Models.Response.V1.AuthenticationService.UserAccess;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class UserActivityResponse: BaseResponse
    {
        [JsonPropertyName("total")]
        [Display(Name = "Total")]
        public int Total { get; set; }

        [JsonPropertyName("data")]
        [Display(Name = "Activity Log")]
        public List<ActivityLogResponse> Data { get; set; } = [];
    }
}

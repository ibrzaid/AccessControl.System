using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserAccess
{
    public class GetAccessHistoryResponse : BaseResponse
    {
        [JsonPropertyName("total")]
        [Display(Name = "Total")]
        public int Total { get; set; }

        [JsonPropertyName("data")]
        [Display(Name = "History")]
        public List<ActivityLogResponse> Data { get; set; } = [];
    }
}

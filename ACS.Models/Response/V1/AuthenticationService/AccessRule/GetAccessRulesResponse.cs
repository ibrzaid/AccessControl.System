using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.AccessRule
{
    public class GetAccessRulesResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "Access Rules")]
        public List<AccessRuleResponse> Data { get; set; } = [];
    }
}

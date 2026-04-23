using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.AccessScope
{
    public class GetAccessScopesResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        [Display(Name = "Scopes")]
        public List<AccessScopeResponse> Data { get; set; } = [];
    }
}

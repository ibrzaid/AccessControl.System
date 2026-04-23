using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserAccessException
{
    public class AccessExceptionResponse
    {
        [JsonPropertyName("access_exception_id")]
        [Display(Name = "Exception ID")]
        public long AccessExceptionId { get; set; }

        [JsonPropertyName("user_access_id")]
        [Display(Name = "Access Rule ID")]
        public long UserAccessId { get; set; }

        [JsonPropertyName("user_id")]
        [Display(Name = "User ID")]
        public int UserId { get; set; }

        [JsonPropertyName("exception_scope_type_id")]
        [Display(Name = "Scope Type")]
        public int ExceptionScopeTypeId { get; set; }

        [JsonPropertyName("exception_resource_id")]
        [Display(Name = "Resource ID")]
        public int ExceptionResourceId { get; set; }

        [JsonPropertyName("is_allowed")]
        [Display(Name = "Type")]
        public bool IsAllowed { get; set; }

        [JsonPropertyName("exception_reason")]
        [Display(Name = "Reason")]
        public string? ExceptionReason { get; set; }

        [JsonPropertyName("created_by")]
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }

        [JsonPropertyName("created_at")]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }
    }
}

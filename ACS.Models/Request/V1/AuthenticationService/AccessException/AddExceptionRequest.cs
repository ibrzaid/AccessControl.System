using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.AuthenticationService.AccessException
{
    public class AddExceptionRequest : BaseRequest
    {
        [JsonPropertyName("user_access_id")]
        [Required(ErrorMessage = "Please provide the access rule ID")]
        [Range(1, long.MaxValue, ErrorMessage = "Please provide a valid access rule ID")]
        [Display(Name = "Access Rule")]
        public long UserAccessId { get; set; }

        [JsonPropertyName("user_id")]
        [Required(ErrorMessage = "Please select a user")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid user")]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [JsonPropertyName("exception_scope_type_id")]
        [Required(ErrorMessage = "Please select an exception scope type")]
        [Range(1, 5, ErrorMessage = "Scope type must be between 1 and 5")]
        [Display(Name = "Exception Scope Type")]
        public int ExceptionScopeTypeId { get; set; }

        [JsonPropertyName("exception_resource_id")]
        [Required(ErrorMessage = "Please select a resource")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid resource")]
        [Display(Name = "Resource")]
        public int ExceptionResourceId { get; set; }

        [JsonPropertyName("is_allowed")]
        [Display(Name = "Exception Type")]
        public bool IsAllowed { get; set; } = false;

        [JsonPropertyName("reason")]
        [StringLength(500, ErrorMessage = "Reason must not exceed 500 characters")]
        [Display(Name = "Reason")]
        public string? Reason { get; set; }
    }
}

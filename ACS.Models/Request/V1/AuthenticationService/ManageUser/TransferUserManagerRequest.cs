

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACS.Models.Request.V1.AuthenticationService.ManageUser
{
    public class TransferUserManagerRequest : BaseRequest
    {
        [JsonPropertyName("managed_user_id")]
        [Required(ErrorMessage = "Please enter a Managed User Id")]
        [Display(Name = "Managed User Id")]
        public int ManagedUserId { get; set; }


        [JsonPropertyName("new_manager_id")]
        [Required(ErrorMessage = "Please enter a New Managed Id")]
        [Display(Name = "New Managed Id")]
        public int NewManagedId { get; set; }


        [JsonPropertyName("notes")]
        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}

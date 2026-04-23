using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class CreateUserDataResponse
    {
        [JsonPropertyName("user_id")]
        [Display(Name = "User ID")]
        public int UserId { get; set; }

        [JsonPropertyName("current_users")]
        [Display(Name = "Current Users")]
        public int CurrentUsers { get; set; }

        [JsonPropertyName("max_users")]
        [Display(Name = "Max Users")]
        public int MaxUsers { get; set; }

        [JsonPropertyName("remaining_slots")]
        [Display(Name = "Remaining Slots")]
        public int RemainingSlots { get; set; }
    }
}

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request.V1.AuthenticationService.ManageUser
{
    public class UpdateUserRequest : BaseRequest
    {
        [JsonPropertyName("full_name")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 255 characters")]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [JsonPropertyName("phone_number")]
        [StringLength(50, ErrorMessage = "Phone number must not exceed 50 characters")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("department")]
        [StringLength(100, ErrorMessage = "Department must not exceed 100 characters")]
        [Display(Name = "Department")]
        public string? Department { get; set; }

        [JsonPropertyName("job_title")]
        [StringLength(100, ErrorMessage = "Job title must not exceed 100 characters")]
        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [JsonPropertyName("timezone")]
        [StringLength(50, ErrorMessage = "Timezone must not exceed 50 characters")]
        [Display(Name = "Timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("language")]
        [StringLength(10, ErrorMessage = "Language code must not exceed 10 characters")]
        [Display(Name = "Language")]
        public string? Language { get; set; }

        [JsonPropertyName("avatar_url")]
        [StringLength(500, ErrorMessage = "Avatar URL must not exceed 500 characters")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "Avatar URL")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("notification_preferences")]
        [Display(Name = "Notification Preferences")]
        public string? NotificationPreferences { get; set; }
    }
}

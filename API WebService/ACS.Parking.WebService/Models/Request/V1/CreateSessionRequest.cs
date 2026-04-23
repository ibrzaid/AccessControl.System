using ACS.Models.Request;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using ACS.Parking.WebService.Attributes.V1;

namespace ACS.Parking.WebService.Models.Request.V1
{
    [ValidatePlateNumberAndCode]
    public class CreateSessionRequest : BaseRequest
    {
       


        [DefaultValue("KSA")]
        [JsonPropertyName("plate_code")]
        [Display(Name = "Plate Code")]
        public string? PlateCode { get; set; }


        [DefaultValue("1234")]
        [JsonPropertyName("plate_number")]
        [Required(ErrorMessage = "Please enter Plate Number")]
        [Display(Name = "Plate Number")]
        public string? PlateNumber { get; set; }


        [DefaultValue(150)]
        [JsonPropertyName("country_id")]
        [Required(ErrorMessage = "Please enter Country Id")]
        [Display(Name = "Country Id")]
        public int Country { get; set; }


        [DefaultValue(1)]
        [JsonPropertyName("state_id")]
        [Required(ErrorMessage = "Please enter State Id")]
        [Display(Name = "State Id")]
        public int State { get; set; }



        [DefaultValue(1)]
        [JsonPropertyName("category_id")]
        [Required(ErrorMessage = "Please enter Category Id")]
        [Display(Name = "Category Id")]
        public int Category { get; set; }


   
        [JsonPropertyName("anpr_id")]
        [Display(Name = "ANPR Id")]
        public int? ANPR { get; set; }



        [DefaultValue("")]
        [JsonPropertyName("camera_capure_url")]
        [Display(Name = "Camera Capure Url")]
        public string? CameraCapureUrl { get; set; }


      
        [JsonPropertyName("subscriber_id")]
        [Display(Name = "Subscriber Id")]
        public int? Subscriber { get; set; }

    }
}

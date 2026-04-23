using System.ComponentModel;
using System.Text.Json.Serialization;
using ACS.Parking.WebService.Attributes.V1;
using System.ComponentModel.DataAnnotations;

namespace ACS.Parking.WebService.Models.Request.V1
{
    [ValidatePlateNumberAndCode]
    public class PaymentSessionRequest : CreateSessionRequest
    {
        [DefaultValue(30)]
        [JsonPropertyName("net_amt")]
        [Display(Name = "Net Amount")]
        [Required(ErrorMessage = "Please enter Net Amount")] 
        public double? NetAmt { get; set; }



        [DefaultValue(4.5)]
        [JsonPropertyName("vat_amt")]
        [Display(Name = "VAT Amount")]
        [Required(ErrorMessage = "Please enter VAT Amount")]
        public double? VATAmt { get; set; }



        [DefaultValue(34.5)]
        [JsonPropertyName("gross_amt")]
        [Display(Name = "Gross Amount")]
        [Required(ErrorMessage = "Please enter Gross Amount")]
        public double? GrossAmt { get; set; }


        [DefaultValue("10")]
        [JsonPropertyName("payment_method")]
        [Display(Name = "Payment Method")]
        [Required(ErrorMessage = "Please enter Payment Method")]
        [RegularExpression("^(10|30|48)$", ErrorMessage = "Payment Method must be 10 (Cash), 30 (Bank Transfer), or 48 (Card).")]
        public string? PaymentMethod { get; set; }


    
        [JsonPropertyName("payment_reference")]
        [Display(Name = "Payment Reference")]
        public string? PaymentReference { get; set; }
    }
}

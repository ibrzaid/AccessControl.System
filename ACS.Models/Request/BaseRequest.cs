using System.Globalization;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Request
{
    /// <summary>
    /// Represents the base class for requests that include geographic coordinates.
    /// </summary>
    /// <remarks>This class provides common properties for specifying latitude and longitude values,  which
    /// are required for requests involving geographic data. Derived classes can extend  this base class to include
    /// additional request-specific properties.</remarks>
    public class BaseRequest : IValidatableObject
    {
        /// <summary>
        /// Latitude as string (input from client)
        /// </summary>
        [DefaultValue("0")]
        [JsonPropertyName("latitude")]
        [Display(Name = "Latitude")]
        [Required(ErrorMessage = "Latitude is required")]
        [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Latitude must be a valid number")]
        public string? Latitude { get; set; }

        /// <summary>
        /// Longitude as string (input from client)
        /// </summary>
        [DefaultValue("0")]
        [JsonPropertyName("longitude")]
        [Display(Name = "Longitude")]
        [Required(ErrorMessage = "Longitude is required")]
        [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Longitude must be a valid number")]
        public string? Longitude { get; set; }



        /// <summary>
        /// Manual validation for ranges
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (!double.TryParse(Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude))
            {
                yield return new ValidationResult(
                    "Latitude must be a valid number",
                    [nameof(Latitude)]);
                yield break;
            }

            if (!double.TryParse(Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude))
            {
                yield return new ValidationResult(
                    "Longitude must be a valid number",
                    [nameof(Longitude)]);
                yield break;
            }

            if (_latitude < -90 || _latitude > 90)
                yield return new ValidationResult(
                    "Latitude must be between -90 and 90",
                    [nameof(Latitude)]);

            if (_longitude < -180 || _longitude > 180)
                yield return new ValidationResult(
                    "Longitude must be between -180 and 180",
                    [nameof(Latitude)]);
        }

    }



    public record BaseRequests(
        [property: DefaultValue("0")]
        [property: JsonPropertyName("latitude")]
        [property: Display(Name = "Latitude")]
        [property: Required(ErrorMessage = "Latitude is required")]
        [property: RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Latitude must be a valid number")]
        string? Latitude,

        [property: DefaultValue("0")]
        [property: JsonPropertyName("longitude")]
        [property: Display(Name = "Longitude")]
        [property: Required(ErrorMessage = "Longitude is required")]
        [property: RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Longitude must be a valid number")]
        string? Longitude
        ) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!double.TryParse(Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude))
            {
                yield return new ValidationResult(
                    "Latitude must be a valid number",
                    [nameof(Latitude)]);
                yield break;
            }

            if (!double.TryParse(Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude))
            {
                yield return new ValidationResult(
                    "Longitude must be a valid number",
                    [nameof(Longitude)]);
                yield break;
            }

            if (_latitude < -90 || _latitude > 90)
                yield return new ValidationResult(
                    "Latitude must be between -90 and 90",
                    [nameof(Latitude)]);

            if (_longitude < -180 || _longitude > 180)
                yield return new ValidationResult(
                    "Longitude must be between -180 and 180",
                    [nameof(Longitude)]);
        }
    }
}

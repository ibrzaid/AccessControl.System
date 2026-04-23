using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace ACS.Models.Attribute.V1
{
    public class LatLongAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var str = value as string;

            if (!double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                return new ValidationResult($"{context.DisplayName} must be a valid number");

            if (context.MemberName!.Contains("Latitude") && (val < -90 || val > 90))
                return new ValidationResult($"{context.DisplayName} must be between -90 and 90");

            if (context.MemberName!.Contains("Longitude") && (val < -180 || val > 180))
                return new ValidationResult($"{context.DisplayName} must be between -180 and 180");

            return ValidationResult.Success;
        }
    }
}

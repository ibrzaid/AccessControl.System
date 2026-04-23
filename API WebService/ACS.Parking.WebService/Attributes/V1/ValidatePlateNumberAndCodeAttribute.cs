using ACS.Parking.WebService.Models.Request.V1;
using ACS.Service.V1.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ACS.Parking.WebService.Attributes.V1
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ValidatePlateNumberAndCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is CreateSessionRequest model)
            {
                var countryService = validationContext.GetService<ICountryService>();
                if (countryService == null) return new ValidationResult( "Validation service not available.",["Country"]);
                try
                {
                    var country = countryService.GetCountryByIdAsync(model.Country).GetAwaiter().GetResult();
                    if (country == null) return new ValidationResult( "Invalid country selected.", ["Country"]);
                    if (!string.IsNullOrEmpty(country.PatternRegex))
                    {
                        string fullPlate = $"{model.PlateCode}{model.PlateNumber}";
                        if (!System.Text.RegularExpressions.Regex.IsMatch(fullPlate, country.PatternRegex))
                            return new ValidationResult($"License plate format is invalid for {country.CountryNames["en-US"]}. Expected format: {country.PatternRegex}",["PlateCode", "PlateNumber"]);                        
                    }
                }
                catch (Exception ex)
                {
                    return new ValidationResult( $"Error validating license plate: {ex.Message}");
                }                
            }
            return ValidationResult.Success;
        }
    }
}

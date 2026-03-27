using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace P3AddNewFunctionalityDotNetCore.Models.Validation
{
    public class NumberGreaterThanZero : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out decimal decimalValue);
            if (decimalValue > 0)
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessageString);

        }
    }
}

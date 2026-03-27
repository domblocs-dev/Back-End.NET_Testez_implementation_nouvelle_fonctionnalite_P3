using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace P3AddNewFunctionalityDotNetCore.Models.Validation
{
    public class DecimalByCultureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Decimal.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal decimalValue);
            if (decimalValue.ToString() == value.ToString())
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessageString);

        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace QBittorrent.CommandLineInterface.Attributes
{
    public class ShareRatioLimitValidationAttribute : ValidationAttribute
    {
        private static readonly string[] Keywords = ["GLOBAL", "NONE"];

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            switch (value)
            {
                case null:
                case string str when Keywords.Contains(str, StringComparer.OrdinalIgnoreCase):
                    return ValidationResult.Success;
                case string str when double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out _):
                    return ValidationResult.Success;
                case string str when double.TryParse(str, out _):
                    return ValidationResult.Success;
                default:
                    return new ValidationResult($"The value {value} is not a correct share ratio limit. " +
                        "The value must be either a number or one of the keywords: GLOBAL, NONE");
            }

        }
    }
}

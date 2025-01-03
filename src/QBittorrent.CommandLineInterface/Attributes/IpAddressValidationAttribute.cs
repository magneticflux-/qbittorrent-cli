using System.ComponentModel.DataAnnotations;
using System.Net;

namespace QBittorrent.CommandLineInterface.Attributes
{
    public class IpAddressValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            switch (value)
            {
                case null:
                case string str when IPAddress.TryParse(str, out _):
                    return ValidationResult.Success;
                default:
                    return new ValidationResult($"The value {value} is not a correct IP address.");
            }

        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace QBittorrent.CommandLineInterface.Attributes
{
    public class IpNetworkValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            switch (value)
            {
                case null:
                case string str when IPNetwork.TryParse(str, out _):
                    return ValidationResult.Success;
                default:
                    return new ValidationResult($"The value {value} is not a correct IP network.");
            }

        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;

namespace QBittorrent.CommandLineInterface.Attributes
{
    public class IpEndpointValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            switch (value)
            {
                case null:
                case string str when TryParse(str.AsSpan(), out _):
                    return ValidationResult.Success;
                default:
                    return new ValidationResult($"The value {value} is not a correct IP endpoint.");
            }

        }

        private static bool TryParse(ReadOnlySpan<char> s, out IPEndPoint? result)
        {
            var addressLength = s.Length; // If there's no port then send the entire string to the address parser
            var lastColonPos = s.LastIndexOf(':');

            // Look to see if this is an IPv6 address with a port.
            if (lastColonPos > 0)
            {
                if (s[lastColonPos - 1] == ']')
                {
                    addressLength = lastColonPos;
                }
                // Look to see if this is IPv4 with a port (IPv6 will have another colon)
                else if (s[..lastColonPos].LastIndexOf(':') == -1)
                {
                    addressLength = lastColonPos;
                }
            }

            if (IPAddress.TryParse(s[..addressLength], out var address))
            {
                uint port = 0;
                if (addressLength == s.Length ||
                    uint.TryParse(s[(addressLength + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out port) && port <= IPEndPoint.MaxPort)

                {
                    result = new IPEndPoint(address, (int)port);
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}

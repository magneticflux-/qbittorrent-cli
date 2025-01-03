using System.Globalization;
using CsvHelper.Configuration;

namespace QBittorrent.CommandLineInterface.Formats
{
    public class CsvFormatOptions
    {
        public string Delimiter { get; set; }

        public char Quote { get; set; }

        public bool Sanitize { get; set; }

        public string Culture { get; set; }

        public static implicit operator CsvConfiguration(CsvFormatOptions options)
        {
            return new CsvConfiguration(string.IsNullOrWhiteSpace(options.Culture)
                ? CultureInfo.InvariantCulture
                : CultureInfo.GetCultureInfo(options.Culture))
            {
                Delimiter = options.Delimiter,
                Quote = options.Quote,
                InjectionOptions = options.Sanitize ? InjectionOptions.Escape : InjectionOptions.None
            };
        }
    }
}

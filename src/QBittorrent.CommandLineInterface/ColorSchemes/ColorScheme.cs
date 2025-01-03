using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace QBittorrent.CommandLineInterface.ColorSchemes
{
    public class ColorScheme
    {
        private const string SchemaResource = "QBittorrent.CommandLineInterface.Schemas.colors-schema.json";
        private const string DarkResource = "QBittorrent.CommandLineInterface.ColorSchemes.dark.json";
        private const string LightResource = "QBittorrent.CommandLineInterface.ColorSchemes.light.json";

        private static ColorScheme? _current;

        static ColorScheme()
        {
            _dark = new Lazy<ColorScheme>(() => JsonConvert.DeserializeObject<ColorScheme>(
                ReadJsonFromResource(DarkResource)) ?? throw new InvalidOperationException());
            _light = new Lazy<ColorScheme>(() => JsonConvert.DeserializeObject<ColorScheme>(
                ReadJsonFromResource(LightResource)) ?? throw new InvalidOperationException());
            _default = new Lazy<ColorScheme>(() => IsLight() ? _light.Value : _dark.Value);

            bool IsLight()
            {
                if (!EnumHelper.IsDefined(Console.BackgroundColor))
                    return false;

                return Console.BackgroundColor == ConsoleColor.White
                    || Console.BackgroundColor == ConsoleColor.Gray
                    || Console.BackgroundColor == ConsoleColor.Yellow
                    || Console.BackgroundColor == ConsoleColor.Cyan
                    || Console.BackgroundColor == ConsoleColor.Magenta;
            }
        }

        public static ColorScheme Dark => _dark.Value;

        public static ColorScheme Light => _light.Value;

        public static ColorScheme Current
        {
            get => _current ?? _default.Value;
            set => _current = value;
        }

        [JsonProperty("normal")]
        public ColorSet Normal { get; private set; }

        [JsonProperty("strong")]
        public ColorSet Strong { get; private set; }

        [JsonProperty("warning")]
        public ColorSet Warning { get; private set; }

        [JsonProperty("active")]
        public ColorSet Active { get; private set; }

        [JsonProperty("inactive")]
        public ColorSet Inactive { get; private set; }

        [JsonProperty("log")]
        public IReadOnlyDictionary<string, ColorSet>? LogColors { get; private set; }

        [JsonProperty("torrent-status")]
        public IReadOnlyDictionary<string, ColorSet>? TorrentStateColors { get; private set; }

        public static async Task<ColorScheme> FromJsonAsync(string json)
        {
            var config = JObject.Parse(json);
            var schema = await LoadSchemaAsync().ConfigureAwait(false);
            var errors = schema.Validate(config);
            if (errors != null && errors.Count != 0)
                throw new Exception("The color scheme file is invalid."); // TODO: Throw specific exception.

            return config.ToObject<ColorScheme>() ?? throw new InvalidOperationException();
        }

        private static async Task<JsonSchema> LoadSchemaAsync()
        {
            var json = ReadJsonFromResource(SchemaResource);
            return await JsonSchema.FromJsonAsync(json).ConfigureAwait(false);
        }

        private static string ReadJsonFromResource(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException())
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                return json;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        // ReSharper disable InconsistentNaming
        private static readonly Lazy<ColorScheme> _dark;
        private static readonly Lazy<ColorScheme> _light;
        private static readonly Lazy<ColorScheme> _default;
        // ReSharper restore InconsistentNaming
    }
}

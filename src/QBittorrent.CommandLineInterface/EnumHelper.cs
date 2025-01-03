using System;

namespace QBittorrent.CommandLineInterface
{
    internal static class EnumHelper
    {
        public static bool IsDefined<T>(T value)
        {
            return Enum.IsDefined(typeof(T), value!);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace QBittorrent.CommandLineInterface
{
    internal static class EnumHelper
    {
        public static bool IsDefined<T>(T value) => Enum.IsDefined(typeof(T), value);
    }
}

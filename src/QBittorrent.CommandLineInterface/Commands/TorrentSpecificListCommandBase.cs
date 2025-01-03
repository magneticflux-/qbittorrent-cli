using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using QBittorrent.CommandLineInterface.Formats;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class TorrentSpecificListCommandBase<T> : TorrentSpecificCommandBase
    {
        private readonly ListFormatter<T> _formatter;

        protected TorrentSpecificListCommandBase()
        {
            _formatter = new ListFormatter<T>(PrintTable, PrintList);
        }

        public virtual string Format { get; set; }

        protected virtual Dictionary<string, Func<object?, object?>>? ListCustomFormatters => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Print(IEnumerable<T> data, bool preferList = false)
        {
            _formatter.PrintFormat(data, Format, preferList);
        }

        protected virtual void PrintTable(IEnumerable<T> list)
        {
            throw new Exception("Unsupported output format.");
        }

        protected virtual void PrintList(IEnumerable<T> list)
        {
            UIHelper.PrintList(list, ListCustomFormatters);
        }
    }
}

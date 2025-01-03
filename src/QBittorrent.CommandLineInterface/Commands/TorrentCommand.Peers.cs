using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Peers))]
    public partial class TorrentCommand
    {
        [Command(Description = "Show the list of torrent peers.", ExtendedHelpText = "[DEPRECATED] Use \"torrent peer list\" instead.")]
        public class Peers : TorrentSpecificCommandBase
        {
            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var response = await client.GetPeerPartialDataAsync(Hash);
                if (response == null)
                    return ExitCodes.Failure;

                var peers = response.PeersChanged?.Values ?? [];

                var doc = new Document(
                    new Grid
                    {
                        Columns =
                        {
                            new Column {Width = GridLength.Auto},
                            new Column {Width = GridLength.Auto},
                            new Column {Width = GridLength.Star(1)},
                            new Column {Width = GridLength.Auto},
                            new Column {Width = GridLength.Auto},
                            new Column {Width = GridLength.Auto},
                            new Column {Width = GridLength.Auto},
                            new Column {Width = GridLength.Auto}
                        },
                        Children =
                        {
                            UIHelper.Header("CC"),
                            UIHelper.Header("Endpoint"),
                            UIHelper.Header("Client"),
                            UIHelper.Header("Progress"),
                            UIHelper.Header("DL Speed", TextAlign.Center),
                            UIHelper.Header("UL Speed", TextAlign.Center),
                            UIHelper.Header("DL", TextAlign.Center, 8),
                            UIHelper.Header("UL", TextAlign.Center, 8),
                            peers.Select(p => new[]
                            {
                                new Cell(p.CountryCode),
                                new Cell(FormatEndpoint(p.Address, p.Port)),
                                new Cell(p.Client),
                                new Cell($"{p.Progress:P0}"),
                                new Cell(FormatSpeed(p.DownloadSpeed).PadLeft(10)),
                                new Cell(FormatSpeed(p.UploadSpeed).PadLeft(10)),
                                new Cell(FormatData(p.Downloaded).PadLeft(8)),
                                new Cell(FormatData(p.Uploaded).PadLeft(8))
                            })
                        },
                        Stroke = LineThickness.Single
                    }
                ).SetColors(ColorScheme.Current.Normal);

                ConsoleRenderer.RenderDocument(doc);

                return ExitCodes.Success;

                string? FormatEndpoint(IPAddress? address, int? port)
                {
                    if (address == null || port == null)
                        return null;

                    return new IPEndPoint(address, port.Value).ToString();
                }

                string FormatSpeed(int? speed)
                {
                    return speed switch
                    {
                        null => string.Empty,
                        < 1024 => $"{speed}  B/s",
                        < 1024 * 1024 => $"{speed / 1024} kB/s",
                        < 1024 * 1024 * 1024 => $"{speed / (1024 * 1024)} MB/s",
                        _ => $"{speed / (1024 * 1024 * 1024)} GB/s"
                    };
                }

                string FormatData(long? amount)
                {
                    return amount switch
                    {
                        null => string.Empty,
                        < 1024 => $"{amount}  B",
                        < 1024 * 1024 => $"{amount / 1024} kB",
                        < 1024 * 1024 * 1024 => $"{amount / (1024 * 1024)} MB",
                        _ => $"{amount / (1024 * 1024 * 1024)} GB"
                    };
                }
            }
        }
    }
}

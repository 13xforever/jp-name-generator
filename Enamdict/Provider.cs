using System.IO.Compression;
using System.Text;

namespace Enamdict;

public static class Provider
{
    // docs: https://www.edrdg.org/enamdict/enamdict_doc.html
    private const string DownloadUrl = "http://ftp.edrdg.org/pub/Nihongo/enamdictu.gz";
    private static readonly Encoding Utf8 = new UTF8Encoding(false);
    private static List<Entry>? Entries = null;
    private static EnamdictInfo? Info = null;

    public static async Task<(EnamdictInfo info, List<Entry> entries)> ParseAsync()
    {
        if (Info is not null && Entries is not null)
            return (Info, Entries);
        
        var fso = new FileStreamOptions
        {
            Mode = FileMode.Open,
            Access = FileAccess.Read,
            Share = FileShare.Read,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
        };
        await using var gzStream = File.Open(@"enamdictu.gz", fso);
        await using var stream = new GZipStream(gzStream, CompressionMode.Decompress);
        using var reader = new StreamReader(stream, Utf8);
        var header = await reader.ReadLineAsync().ConfigureAwait(false);
        Info = EnamdictInfo.Parse(header);
        var entries = new List<Entry>();
        while (await reader.ReadLineAsync().ConfigureAwait(false) is {Length: >0} line)
            entries.AddRange(Entry.Parse(line));
        Entries = entries;
        return (Info, Entries);
    }
}
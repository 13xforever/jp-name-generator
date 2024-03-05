using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;

namespace Enamdict;

public static class Provider
{
    // docs: https://www.edrdg.org/enamdict/enamdict_doc.html
    private const string DownloadUrl = "http://ftp.edrdg.org/pub/Nihongo/enamdictu.gz";
    private static readonly Encoding Utf8 = new UTF8Encoding(false);
    private static List<Entry>? Entries = null;
    private static EnamdictInfo? Info = null;

    private static readonly string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "jp-name-generator"
    );
    private static readonly string dicName = Path.GetFileName(DownloadUrl);
    private static readonly string dicPath = Path.Combine(AppDataPath, dicName);
    private static readonly HttpClient httpClient = new()
    {
        DefaultRequestHeaders =
        {
            UserAgent = { new ProductInfoHeaderValue("JpNameGenerator", "1.0") }
        }
    };

    private static readonly SemaphoreSlim fence = new(1, 1);
    
    public static async Task<(EnamdictInfo info, List<Entry> entries)> ParseAsync()
    {
        await fence.WaitAsync().ConfigureAwait(false);
        try
        {
            if (Info is not null && Entries is not null)
                return (Info, Entries);
        }
        finally
        {
            fence.Release();
        }

        var isCached = File.Exists(dicPath);
        if (!isCached)
            await UpdateDicAsync().ConfigureAwait(false);
        
        await fence.WaitAsync().ConfigureAwait(false);
        try
        {
            var fso = new FileStreamOptions
            {
                Mode = FileMode.Open,
                Access = FileAccess.Read,
                Share = FileShare.Read,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
            };
            await using var gzStream = File.Open(dicPath, fso);
            await using var stream = new GZipStream(gzStream, CompressionMode.Decompress);
            using var reader = new StreamReader(stream, Utf8);
            var header = await reader.ReadLineAsync().ConfigureAwait(false);
            if (header is null)
                throw new FormatException($"Failed to parse {dicName}");
            
            Info = EnamdictInfo.Parse(header);
            var entries = new List<Entry>();
            while (await reader.ReadLineAsync().ConfigureAwait(false) is { Length: > 0 } line)
                entries.AddRange(Entry.Parse(line));
            Entries = entries;
            if (isCached)
                UpdateInBgAsync();
            return (Info, Entries);
        }
        finally
        {
            fence.Release();
        }
    }

    private static async void UpdateInBgAsync()
    {
        try { await UpdateDicAsync().ConfigureAwait(false); } catch { }
    } 
    
    private static async Task UpdateDicAsync()
    {
        var cachedSize = 0L;
        try
        {
            if (File.Exists(dicPath))
                cachedSize = new FileInfo(dicPath).Length;
            using var request = new HttpRequestMessage(HttpMethod.Head, DownloadUrl);
            using var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            if (response.Content.Headers.ContentLength == cachedSize)
                return;
            
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);
        } catch { }
        
        var tmpName = $"new_{dicName}";
        var tmpPath = Path.Combine(AppDataPath, tmpName);
        try
        {
            var fso = new FileStreamOptions
            {
                Mode = FileMode.Create,
                Access = FileAccess.Write,
                Share = FileShare.Read,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
            };
            await using var downloadStream = await httpClient.GetStreamAsync(DownloadUrl).ConfigureAwait(false);
            await using var fileStream = File.Open(tmpPath, fso);
            await downloadStream.CopyToAsync(fileStream).ConfigureAwait(false);
            await fileStream.FlushAsync().ConfigureAwait(false);
        } catch {}

        try
        {
            File.Move(tmpPath, dicPath, true);
        } catch {}

        await fence.WaitAsync().ConfigureAwait(false);
        Info = null;
        Entries = null;
        fence.Release();
    }
}
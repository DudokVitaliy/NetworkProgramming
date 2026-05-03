using System.IO;
using System.Net.Http;

public class DownloadService
{
    private HttpClient client = new HttpClient();

    public async Task DownloadFileAsync(
        string url,
        string path,
        IProgress<int> progress,
        CancellationToken token)
    {
        using var response = await client.GetAsync(url,
            HttpCompletionOption.ResponseHeadersRead);

        var total = response.Content.Headers.ContentLength ?? -1;

        using var stream = await response.Content.ReadAsStreamAsync();
        using var file = new FileStream(path, FileMode.Create);

        var buffer = new byte[8192];
        long read = 0;

        while (true)
        {
            int bytes = await stream.ReadAsync(buffer, 0, buffer.Length, token);

            if (bytes == 0) break;

            await file.WriteAsync(buffer, 0, bytes);

            read += bytes;

            if (total > 0)
            {
                int percent = (int)((read * 100) / total);
                progress.Report(percent);
            }
        }
    }
}
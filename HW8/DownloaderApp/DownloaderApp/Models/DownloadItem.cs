public class DownloadItem
{
    public string Url { get; set; }
    public string FileName { get; set; }
    public long TotalBytes { get; set; }
    public long DownloadedBytes { get; set; }
    public string Status { get; set; }
}
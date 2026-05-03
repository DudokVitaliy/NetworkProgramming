using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DownloaderApp
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cts;
        private string savePath;

        public ObservableCollection<DownloadItem> Downloads { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Downloads = new ObservableCollection<DownloadItem>();
            DownloadsGrid.ItemsSource = Downloads;
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            if (dialog.ShowDialog() == true)
            {
                savePath = dialog.FileName;
            }
        }

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            string url = UrlTextBox.Text;

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(savePath))
            {
                MessageBox.Show("Enter URL and choose save file!");
                return;
            }

            cts = new CancellationTokenSource();

            var item = new DownloadItem
            {
                Url = url,
                FileName = Path.GetFileName(savePath),
                Status = "Starting..."
            };

            Downloads.Add(item);

            try
            {
                await DownloadFileAsync(url, savePath, item, cts.Token);
                item.Status = "Completed";
            }
            catch (OperationCanceledException)
            {
                item.Status = "Cancelled";
            }
            catch (Exception ex)
            {
                item.Status = "Error";
                MessageBox.Show(ex.Message);
            }

            DownloadsGrid.Items.Refresh();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            cts?.Cancel();
        }

        private async Task DownloadFileAsync(
            string url,
            string path,
            DownloadItem item,
            CancellationToken token)
        {
            using HttpClient client = new HttpClient();

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;

            using var input = await response.Content.ReadAsStreamAsync();
            using var output = new FileStream(path, FileMode.Create);

            byte[] buffer = new byte[8192];
            long totalRead = 0;
            int read;

            while ((read = await input.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
            {
                await output.WriteAsync(buffer, 0, read, token);

                totalRead += read;

                if (totalBytes > 0)
                {
                    int progress = (int)((totalRead * 100) / totalBytes);
                    item.Status = $"Downloading {progress}%";
                    DownloadsGrid.Items.Refresh();
                }
            }
        }
    }
}
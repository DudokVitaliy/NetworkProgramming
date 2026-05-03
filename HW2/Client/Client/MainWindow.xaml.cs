using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    public partial class MainWindow : Window
    {
        UdpClient client;
        IPEndPoint server;

        ObservableCollection<string> messages = new ObservableCollection<string>();

        string nick = "";
        bool listening = false;

        public MainWindow()
        {
            InitializeComponent();

            client = new UdpClient();
            server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4040);

            DataContext = messages;
        }
        private void msgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendBtn(sender, e);
            }
        }
        private void JoinBtn(object sender, RoutedEventArgs e)
        {
            nick = nickTextBox.Text.Trim();

            if (string.IsNullOrEmpty(nick))
            {
                MessageBox.Show("Enter nickname");
                return;
            }

            Send($"JOIN|{nick}");

            if (!listening)
            {
                listening = true;
                _ = Listen(); // важливо: без await в UI thread
            }
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            string text = msgTextBox.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Empty message");
                return;
            }

            if (string.IsNullOrEmpty(nick))
            {
                MessageBox.Show("Join first!");
                return;
            }

            Send($"MSG|{nick}|{text}");
            msgTextBox.Clear();
        }

        private void LeaveBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                Send($"LEAVE|{nick}");
            }
            catch { }

            listening = false;

            client.Close();

            Application.Current.Shutdown();
        }

        private async Task Listen()
        {
            try
            {
                while (listening)
                {
                    var data = await client.ReceiveAsync();
                    string msg = Encoding.Unicode.GetString(data.Buffer);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        messages.Add(msg);
                    });
                }
            }
            catch (ObjectDisposedException)
            {
                // норм при Close()
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Listener error: {ex.Message}");
            }
        }

        private void Send(string msg)
        {
            byte[] data = Encoding.Unicode.GetBytes(msg);
            client.Send(data, data.Length, server);
        }
    }
}
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        TcpClient client;
        NetworkStream stream;
        string name;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectBtn(object sender, RoutedEventArgs e)
        {
            name = nameBox.Text;

            client = new TcpClient("127.0.0.1", 5000);
            stream = client.GetStream();

            Send("JOIN|" + name);

            Thread t = new Thread(Listen);
            t.Start();
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            string text = msgBox.Text;

            if (string.IsNullOrWhiteSpace(text)) return;

            string time = DateTime.Now.ToShortTimeString();

            Send($"MSG|{name}|{text}|{time}");

            msgBox.Clear();
        }

        private void Listen()
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int size = stream.Read(buffer, 0, buffer.Length);
                    string msg = Encoding.UTF8.GetString(buffer, 0, size);

                    Dispatcher.Invoke(() =>
                    {
                        chatBox.Items.Add(msg);
                    });
                }
                catch
                {
                    break;
                }
            }
        }

        private void Send(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            stream.Write(data, 0, data.Length);
        }
    }
}
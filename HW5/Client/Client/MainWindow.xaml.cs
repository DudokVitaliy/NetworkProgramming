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
            name = nameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Enter name!");
                return;
            }

            client = new TcpClient("127.0.0.1", 5000);
            stream = client.GetStream();

            Send($"JOIN|{name}");

            Thread t = new Thread(Listen);
            t.IsBackground = true;
            t.Start();
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            string text = msgBox.Text;

            if (string.IsNullOrWhiteSpace(text))
                return;

            string time = DateTime.Now.ToShortTimeString();

            Send($"MSG|{name}|{time}|{text}");

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

                    string[] parts = msg.Split('|');

                    if (parts[0] == "FULL")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Server full!");
                            Close();
                        });
                        return;
                    }

                    if (parts[0] == "MSG")
                    {
                        string show = $"{parts[1]} [{parts[2]}]: {parts[3]}";

                        Dispatcher.Invoke(() =>
                        {
                            chatBox.Items.Add(show);
                        });
                    }
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
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        UdpClient client;
        IPEndPoint server;

        public MainWindow()
        {
            InitializeComponent();

            client = new UdpClient();
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            string ip = ipBox.Text;
            int port = int.Parse(portBox.Text);

            server = new IPEndPoint(IPAddress.Parse(ip), port);

            string text = msgBox.Text;

            if (string.IsNullOrWhiteSpace(text))
                return;

            Send("MSG|" + text);
            msgBox.Clear();

            Listen();
        }

        private async void Listen()
        {
            var result = await client.ReceiveAsync();
            string msg = Encoding.UTF8.GetString(result.Buffer);

            if (msg.StartsWith("RESP|"))
            {
                string answer = msg.Substring(5);

                Dispatcher.Invoke(() =>
                {
                    chatBox.Items.Add("Bot: " + answer + "   " + DateTime.Now.ToShortTimeString());
                });
            }
        }

        private void Send(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            client.Send(data, data.Length, server);
        }
    }
}
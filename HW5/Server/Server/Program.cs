using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static TcpListener server;
    static List<TcpClient> clients = new List<TcpClient>();
    static Dictionary<TcpClient, string> names = new Dictionary<TcpClient, string>();

    static int maxClients = 3;

    static void Main()
    {
        server = new TcpListener(IPAddress.Any, 5000);
        server.Start();

        Console.WriteLine("Server started...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();

            if (clients.Count >= maxClients)
            {
                Send(client, "FULL|Server full");
                client.Close();
                continue;
            }

            clients.Add(client);

            Thread t = new Thread(HandleClient);
            t.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int size = stream.Read(buffer, 0, buffer.Length);
                if (size == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, size);

                string[] parts = msg.Split('|');

                if (parts[0] == "JOIN")
                {
                    names[client] = parts[1];
                    Send(client, "OK|Welcome");
                    Broadcast($"MSG|Server|{Time()}|{parts[1]} joined");
                }
                else if (parts[0] == "MSG")
                {
                    string name = parts[1];
                    string text = parts[2];

                    Broadcast($"MSG|{name}|{Time()}|{text}");
                }
                else if (parts[0] == "LEAVE")
                {
                    break;
                }
            }
        }
        catch { }

        Disconnect(client);
    }

    static void Broadcast(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        foreach (var c in clients)
        {
            try
            {
                c.GetStream().Write(data, 0, data.Length);
            }
            catch { }
        }
    }

    static void Send(TcpClient client, string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        client.GetStream().Write(data, 0, data.Length);
    }

    static void Disconnect(TcpClient client)
    {
        if (clients.Contains(client))
            clients.Remove(client);

        if (names.ContainsKey(client))
        {
            Broadcast($"MSG|Server|{Time()}|{names[client]} left");
            names.Remove(client);
        }

        client.Close();
    }

    static string Time()
    {
        return DateTime.Now.ToShortTimeString();
    }
}
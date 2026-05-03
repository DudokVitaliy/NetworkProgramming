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

    static void Main()
    {
        server = new TcpListener(IPAddress.Any, 5000);
        server.Start();

        Console.WriteLine("Server started...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);

            Console.WriteLine("Client connected");

            Thread t = new Thread(HandleClient);
            t.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];

        while (true)
        {
            try
            {
                int size = stream.Read(buffer, 0, buffer.Length);
                if (size == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, size);

                Console.WriteLine("Received: " + msg);

                Broadcast(msg);
            }
            catch
            {
                break;
            }
        }

        clients.Remove(client);
        client.Close();
    }

    static void Broadcast(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        foreach (var client in clients)
        {
            try
            {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch { }
        }
    }
}
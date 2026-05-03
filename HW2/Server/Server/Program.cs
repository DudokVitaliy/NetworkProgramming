using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ChatServer
{
    const int PORT = 4040;

    private UdpClient server;
    private IPEndPoint remote;

    private Dictionary<IPEndPoint, string> users = new Dictionary<IPEndPoint, string>();

    public ChatServer()
    {
        server = new UdpClient(PORT);
        remote = new IPEndPoint(IPAddress.Any, 0);
    }

    public void Start()
    {
        Console.WriteLine("Server started...");

        while (true)
        {
            byte[] data = server.Receive(ref remote);
            string msg = Encoding.Unicode.GetString(data);

            HandleMessage(msg, remote);
        }
    }

    private void HandleMessage(string msg, IPEndPoint client)
    {
        string[] parts = msg.Split('|');

        switch (parts[0])
        {
            case "JOIN":
                Join(client, parts[1]);
                break;

            case "LEAVE":
                Leave(client);
                break;

            case "MSG":
                SendAll($"{parts[1]}: {parts[2]}");
                break;
        }
    }

    private void Join(IPEndPoint client, string nick)
    {
        if (!users.ContainsKey(client))
            users.Add(client, nick);

        Console.WriteLine($"{nick} joined");

        SendAll($"[SYSTEM] {nick} joined chat");
    }

    private void Leave(IPEndPoint client)
    {
        if (users.ContainsKey(client))
        {
            string nick = users[client];
            users.Remove(client);

            Console.WriteLine($"{nick} left");

            SendAll($"[SYSTEM] {nick} left chat");
        }
    }

    private void SendAll(string message)
    {
        byte[] data = Encoding.Unicode.GetBytes(message);

        foreach (var user in users.Keys)
        {
            server.Send(data, data.Length, user);
        }
    }
}

class Program
{
    static void Main()
    {
        new ChatServer().Start();
    }
}
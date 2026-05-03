using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static List<string> answers = new List<string>()
    {
        "Fine!",
        "Hello!",
        "I'm listening...",
        "Interesting...",
        "Could you explain more?",
        "OK 👍",
        "I don't understand, but sounds good"
    };

    static void Main()
    {
        UdpClient server = new UdpClient(8080);
        IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);

        Console.WriteLine("Server started...");

        while (true)
        {
            byte[] data = server.Receive(ref client);
            string msg = Encoding.UTF8.GetString(data);

            Console.WriteLine("Received: " + msg);

            string response = GenerateResponse(msg);

            byte[] outData = Encoding.UTF8.GetBytes("RESP|" + response);

            server.Send(outData, outData.Length, client);
        }
    }

    static string GenerateResponse(string msg)
    {
        msg = msg.ToLower();

        if (msg.Contains("hello"))
            return "Hello 👋";

        if (msg.Contains("how"))
            return "I'm fine, thanks!";

        if (msg.Contains("name"))
            return "I'm UDP Bot 🤖";

        // fallback random
        Random rnd = new Random();
        return answers[rnd.Next(answers.Count)];
    }
}
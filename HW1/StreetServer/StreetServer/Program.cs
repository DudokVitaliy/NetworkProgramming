using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace StreetServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server started...");

            TcpListener listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();

            Console.WriteLine("Waiting for client...");

            while (true)
            {
                try
                {
                    Socket client = listener.AcceptSocket();
                    Console.WriteLine("Client connected");

                    byte[] buffer = new byte[1024];

                    int size = client.Receive(buffer);
                    string index = Encoding.UTF8.GetString(buffer, 0, size);

                    Console.WriteLine($"Received index: {index}");

                    string response = FindStreets(index);

                    client.Send(Encoding.UTF8.GetBytes(response));

                    Console.WriteLine($"Sent: {response}");

                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static string FindStreets(string index)
        {
            try
            {
                string filePath = "data.txt";

                if (!File.Exists(filePath))
                    return "Data file not found";

                foreach (var line in File.ReadAllLines(filePath))
                {
                    var parts = line.Split(':');

                    if (parts.Length != 2)
                        continue;

                    if (parts[0] == index)
                        return parts[1];
                }

                return "Not found";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
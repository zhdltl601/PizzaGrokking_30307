using System.Net.Sockets;
using System.Text;

namespace _99_BusyClient
{
    internal class BusyClient
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                new Thread(() =>
                {
                    try
                    {
                        using var client = new TcpClient("127.0.0.1", 12345);
                        var stream = client.GetStream();
                        var msg = Encoding.UTF8.GetBytes("5\n");
                        stream.Write(msg, 0, msg.Length);

                        byte[] buffer = new byte[1024];
                        int read = stream.Read(buffer, 0, buffer.Length);
                        string response = Encoding.UTF8.GetString(buffer, 0, read);
                        Console.WriteLine(response.Trim());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }).Start();
            }
        }
    }
}

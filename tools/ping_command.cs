using System.Net.Sockets;
using System.Text;

public static class PingCommand
{
    public static void Main(string[] args)
    {
        const string command = "*1\r\n$4\r\nPING\r\n";

        using var client = new TcpClient();
        client.Connect("127.0.0.1", 6379);

        using var stream = client.GetStream();
        var request = Encoding.ASCII.GetBytes(command);
        stream.Write(request, 0, request.Length);
        stream.Flush();

        var buffer = new byte[256];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);

        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));
    }
}

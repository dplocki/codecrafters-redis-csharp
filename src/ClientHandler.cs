using System.Net.Sockets;
using System.Text;

public class ClientHandler
{
    private readonly RESPParser parser = new();

    public void Handle(Socket client)
    {
        var receiveBuffer = new byte[1024];
        while (true)
        {
            int bytesReceived = client.Receive(receiveBuffer);
            if (bytesReceived == 0)
            {
                break;
            }

            var result = parser.Parse(receiveBuffer);
            if (result is RespBulkString bulkString)
            {
                var command = Encoding.UTF8.GetString(bulkString.Value!);
                var args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (args[0].Equals("PING", StringComparison.OrdinalIgnoreCase))
                {
                    client.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));
                }
                else
                {
                    client.Send(Encoding.UTF8.GetBytes("-ERR unknown command\r\n"));
                }
            }
            else
            {
                client.Send(Encoding.UTF8.GetBytes("-ERR invalid request\r\n"));
            }
        }

        client.Close();
    }
}
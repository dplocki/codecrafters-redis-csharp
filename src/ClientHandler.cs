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
                    var response = new string[args.Length];
                    response[0] = "PONG";
                  
                    client.Send(RESPSerializer.Serialize(new RespBulkString(Encoding.UTF8.GetBytes(string.Join("\r\n", response) + "\r\n"))));
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
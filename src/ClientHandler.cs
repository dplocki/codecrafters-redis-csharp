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
            if (result is RespArray { Values: { Count: > 0 } values })
            {
                var command = values[0] as RespBulkString;
                if (command != null && Encoding.UTF8.GetString(command.Value!).Equals("PING", StringComparison.OrdinalIgnoreCase))
                {
                    RespValue response = values.Count switch
                    {
                        1 => new RespSimpleString("PONG"),
                        2 when values[1] is RespBulkString message => message,
                        _ => new RespError("ERR wrong number of arguments for 'ping' command"),
                    };
                    client.Send(RESPSerializer.Serialize(response));
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
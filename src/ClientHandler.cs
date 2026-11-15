using System.Net.Sockets;
using System.Text;

public class ClientHandler
{
    private RESPParser parser = new RESPParser();

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
            if (result is RespArray array && array.Values.Count > 0)
            {
                var command = array.Values[0] as RespBulkString;
                if (command != null && Encoding.UTF8.GetString(command.Value).Equals("PING", StringComparison.OrdinalIgnoreCase))
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
            
            client.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));
        }

        client.Close();
    }
}
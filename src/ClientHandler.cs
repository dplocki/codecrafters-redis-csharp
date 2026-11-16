using System.Net.Sockets;
using System.Text;

public class ClientHandler
{
    public void Handle(Socket client)
    {
        var receiveBuffer = new byte[1024];
        var pendingBytes = new List<byte>();
        var parser = new RESPParser();

        try
        {
            while (true)
            {
                int bytesReceived = client.Receive(receiveBuffer);
                if (bytesReceived == 0)
                {
                    break;
                }

                pendingBytes.AddRange(new ArraySegment<byte>(receiveBuffer, 0, bytesReceived));

                while (pendingBytes.Count > 0)
                {
                    RespValue? request;
                    int bytesConsumed;
                    try
                    {
                        if (!parser.TryParse([.. pendingBytes], out request, out bytesConsumed))
                        {
                            break;
                        }
                    }
                    catch (FormatException)
                    {
                        SendAll(client, Encoding.UTF8.GetBytes("-ERR invalid request\r\n"));
                        return;
                    }

                    pendingBytes.RemoveRange(0, bytesConsumed);
                    SendAll(client, CreateResponse(request!));
                }
            }
        }
        finally
        {
            client.Close();
        }
    }

    private static byte[] CreateResponse(RespValue request)
    {
        if (request is not RespBulkString { Value: not null } bulkString)
        {
            return Encoding.UTF8.GetBytes("-ERR invalid request\r\n");
        }

        var command = Encoding.UTF8.GetString(bulkString.Value);
        var args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 0)
        {
            return Encoding.UTF8.GetBytes("-ERR invalid request\r\n");
        }

        if (args[0].Equals("PING", StringComparison.OrdinalIgnoreCase))
        {
            return Encoding.UTF8.GetBytes("+PONG\r\n");
        }

        if (args[0].Equals("ECHO", StringComparison.OrdinalIgnoreCase) && args.Length == 2)
        {
            return RESPSerializer.Serialize(new RespBulkString(Encoding.UTF8.GetBytes(args[1])));
        }

        return Encoding.UTF8.GetBytes("-ERR unknown command\r\n");
    }

    private static void SendAll(Socket client, byte[] response)
    {
        int sent = 0;
        while (sent < response.Length)
        {
            int bytesSent = client.Send(response, sent, response.Length - sent, SocketFlags.None);
            if (bytesSent == 0)
            {
                throw new SocketException((int)SocketError.ConnectionReset);
            }

            sent += bytesSent;
        }
    }
}
using System.Net.Sockets;
using System.Text;

public class ClientHandler
{
    private readonly Memory memory = new();

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

    private byte[] CreateResponse(RespValue request)
    {
        var args = ExtractCommandArgs(request);
        if (args.Length == 0)
        {
            return Encoding.UTF8.GetBytes("-ERR invalid request\r\n");
        }

        var command = args[0].ToUpper();
        if (command == "PING")
        {
            return Encoding.UTF8.GetBytes("+PONG\r\n");
        }

        if (command == "ECHO" && args.Length == 2)
        {
            return RESPSerializer.Serialize(new RespBulkString(Encoding.UTF8.GetBytes(args[1])));
        }

        if (command == "GET" && args.Length == 2)
        {
            var value = memory.Get(args[1]);
            if (value is null)
            {
                return Encoding.UTF8.GetBytes("$-1\r\n");
            }

            return RESPSerializer.Serialize(new RespBulkString(Encoding.UTF8.GetBytes(value)));
        }

        if (command == "SET" && args.Length >= 3)
        {
            TimeSpan expire;

            if (args.Length == 5)
            {
                if (args[3].Equals("PX", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(args[4], out int milliseconds))
                    {
                        expire = TimeSpan.FromMilliseconds(milliseconds);
                    }
                    else
                    {
                        return Encoding.UTF8.GetBytes("-ERR invalid PX value\r\n");
                    }
                }
                else if (args[3].Equals("EX", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(args[4], out int seconds))
                    {
                        expire = TimeSpan.FromSeconds(seconds);
                    }
                    else
                    {
                        return Encoding.UTF8.GetBytes("-ERR invalid EX value\r\n");
                    }
                }
                else
                {
                    return Encoding.UTF8.GetBytes("-ERR unknown option\r\n");
                }
            }
            else
            {
                // default expiration time if not provided
                expire = TimeSpan.FromMilliseconds(100);
            }

            memory.Set(args[1], args[2], expire);
            return Encoding.UTF8.GetBytes("+OK\r\n");
        }

        return Encoding.UTF8.GetBytes("-ERR unknown command\r\n");
    }

    private static string[] ExtractCommandArgs(RespValue request)
    {
        if (request is RespArray { Values: not null } array)
        {
            var values = new List<string>();
            foreach (var item in array.Values)
            {
                if (item is not RespBulkString { Value: not null } arrayBulkValue)
                {
                    return [];
                }

                values.Add(Encoding.UTF8.GetString(arrayBulkValue.Value));
            }

            return [.. values];
        }

        if (request is RespBulkString { Value: not null } singleBulkValue)
        {
            return [Encoding.UTF8.GetString(singleBulkValue.Value)];
        }

        return [];
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
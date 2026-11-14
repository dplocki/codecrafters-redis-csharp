using System.Net;
using System.Net.Sockets;
using System.Text;

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();

while (true)
{
    var client = server.AcceptSocket();
    _ = Task.Run(() => HandleClient(client));
}

static void HandleClient(Socket client)
{
    var recieveBuffer = new byte[1024];
    while (true)
    {
        if (client.Receive(recieveBuffer) == 0) {
            break;
        }

        client.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));
    }

    client.Close();
}

using System.Net;
using System.Net.Sockets;

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();
var clientHandler = new ClientHandler();

while (true)
{
    var client = server.AcceptSocket();
    _ = Task.Run(() => clientHandler.Handle(client));
}

using System.Net;
using System.Net.Sockets;
using System.Text;

// Uncomment the code below to pass the first stage
var server = new TcpListener(IPAddress.Any, 6379);
server.Start();

var client = server.AcceptSocket();
client.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));


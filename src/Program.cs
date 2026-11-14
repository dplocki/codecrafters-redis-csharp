using System.Net;
using System.Net.Sockets;

// Uncomment the code below to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();

Socket client = server.AcceptSocket();
client.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));

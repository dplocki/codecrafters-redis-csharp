using System.Net;
using System.Net.Sockets;

// Uncomment the code below to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();
server.AcceptSocket(); // wait for client

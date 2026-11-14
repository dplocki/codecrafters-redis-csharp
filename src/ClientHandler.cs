using System.Net.Sockets;
using System.Text;

public class ClientHandler
{
    public void Handle(Socket client)
    {
        var receiveBuffer = new byte[1024];
        while (true)
        {
            if (client.Receive(receiveBuffer) == 0)
            {
                break;
            }

            client.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));
        }

        client.Close();
    }
}
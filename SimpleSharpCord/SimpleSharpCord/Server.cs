using System.Net.Sockets;
using System.Net;
using System.Text;

public class Server
{
    TcpListener server;
    List<TcpClient> clients = new List<TcpClient>();
    Thread dataHandleThread;
    public static void Main(string[] args)
    {
        Server main = new Server();

        //Console.WriteLine("Please enter a HOST IP");

        //IPAddress ip = IPAddress.Parse(Console.ReadLine());

        main.StartServer(/*ip*/);
        while (true)
        {

        }
    }
    public void StartServer(/*IPAddress ip*/)
    {
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 12346);
        server.Start();

        AcceptConnections();
    }
    public void AcceptConnections()
    {
        server.BeginAcceptTcpClient(HandleConnections, server);
    }
    public void HandleConnections(IAsyncResult result)
    {
        AcceptConnections();
        TcpClient client = server.EndAcceptTcpClient(result);

        if (!clients.Contains(client))
        {
            clients.Add(client);
        }

        NetworkStream stream = client.GetStream();

        dataHandleThread = new Thread(()=> HandleData(stream, client));
        dataHandleThread.Start();
    }
    void HandleData(NetworkStream stream, TcpClient _client)
    {
        int recieved;
        byte[] readBuffer = new byte[1024];
        while (_client.Connected)
        {
            while((recieved = stream.Read(readBuffer)) != 0)
            {
                string msg = Encoding.UTF8.GetString(readBuffer, 0, recieved);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(msg);
                foreach (var client in clients)
                {
                    if(client != _client)
                    {
                        client.Client.Send(sendBuffer);
                    }
                }
            }
        }
    }
    public void EndServer()
    {
        server.Stop();
    }
}
public static class PacketBuilder
{
    public static byte[] BuildStringPacket(string msg)
    {
        return Encoding.UTF8.GetBytes(msg);
    }
}
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

public class Client
{
    TcpClient client;
    string name;
    public static void Main(string[] args)
    {
        Client client = new Client();
        //Console.WriteLine("Input an IP to connect to");

        //IPAddress ip = IPAddress.Parse(Console.ReadLine());
        Console.WriteLine("You can write '/exit to close the program");
        Console.WriteLine("Input your name");

        client.StartClient(/*ip*/Console.ReadLine());
    }
    public void StartClient(/*IPAddress ip*/string _name)
    {
        name = _name;

        client = new TcpClient();
        client.Connect(IPAddress.Parse("127.0.0.1"), 12346);

        while (!client.Connected)
        {

        }
        HandleClient();
    }
    void HandleClient()
    {
        NetworkStream stream = client.GetStream();

        Thread writeThread = new Thread(()=>SendData(stream));
        Thread readThread = new Thread(()=> RecieveData(stream));

        writeThread.Start();
        readThread.Start();
    }
    void SendData(NetworkStream stream)
    {
        while (true)
        {
            string? msg = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(msg))
            {
                if (msg == "/exit")
                {
                    EndClient();
                    Environment.Exit(0);
                    return;
                }
                byte[] msgBuffer = Encoding.UTF8.GetBytes($"{name}: {msg}");
                stream.Write(msgBuffer);
            }
        }
    }
    void RecieveData(NetworkStream stream)
    {
        int recieved;
        byte[] readBuffer = new byte[1024];

        while (true)
        {
            while ((recieved = stream.Read(readBuffer)) != 0)
            {
                string msg = Encoding.UTF8.GetString(readBuffer, 0, recieved);
                Console.WriteLine(msg);
            }
        }
    }
    void EndClient()
    {
        client.Dispose();
        client.Close();
    }
}
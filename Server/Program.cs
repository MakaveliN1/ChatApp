using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{

    static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
    static void Main(string[] args)
    {
        
        int count = 1;
      
        TcpListener listener = new TcpListener(IPAddress.Any, 5479);
        listener.Start();
        Console.WriteLine("Server started and listening on Port 5479");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();

            list_clients.Add(count, client);
            Console.WriteLine("A Client Connected to Server");
            broadcast("A new user joined Chat");
            Thread t = new Thread(handle_clients);
            t.Start(count);
            count++;
        }
    }

    public static void handle_clients(object o)
    {
        int id = (int)o;
        TcpClient client;
        client = list_clients[id];
        while (true)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int byte_count = stream.Read(buffer, 0, buffer.Length);

            if (byte_count == 0)
            {
                break;
            }

            string data = Encoding.ASCII.GetString(buffer, 0, byte_count);
            broadcast(data);
            Console.WriteLine(data);
        }
       
       
        list_clients.Remove(id);
        client.Client.Shutdown(SocketShutdown.Both);
        client.Close();
        Console.WriteLine("A Client Disconnected from Server");
        broadcast("A user left Chat");
    }

    public static void broadcast(string data)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(data);

        foreach (TcpClient c in list_clients.Values)
        {
            NetworkStream stream = c.GetStream();

            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
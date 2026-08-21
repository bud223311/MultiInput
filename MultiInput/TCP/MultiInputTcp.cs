using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using Microsoft.VisualBasic;
using MultiInput.Enums;
using MultiInput.Enums.Constants;
using MultiInput.Inputter;

namespace MultiInput.TCP;

public class MultiInputTcp
{
    
    public class KTcpHost(TcpListener listener)
    {
        public TcpListener Listener = listener;
        public Socket? Client;
        private bool _connected;

        public void Awake(){
            Console.WriteLine($"Waiting For Connections...");
            _connected = false;
            Listener.Start();
            Key.InitializeKeys();
            
        }
        public void Update(){
            if (Listener.Pending()) {
                Console.WriteLine($"Found Pending Request");
                Client = Listener.AcceptSocket();
            }
            if (Client is null||!Client.Connected) {
                _connected = false;
                return;
            }
            if (!_connected) {
                OnConnect(Listener.Server.RemoteEndPoint);
                _connected = true;
            }
            Key.UpdateKeys(Client);
        }
    }

    public class KTcpClient(TcpClient client,IPAddress connectionTo,int port)
    {
        public TcpClient Client = client;
        public IPAddress ConnectionGoesTo = connectionTo;
        public int Port = port;
        private bool _connected;
        public void Awake(){
            _connected = false;
            Console.WriteLine($"Connecting...");
            Client.Connect(ConnectionGoesTo, Port);
        }
        public void Update(){
            if (Client is{ Connected: false, Available: 0}) {
                _connected = false;
                return;
            }

            if (!_connected) {
                OnConnect(Client.Client.RemoteEndPoint);
                _connected = true;
            }



            byte[] buffer = new byte[255];
            int amount = Client.Client.Receive(buffer);
            
            if (amount is 0) {
                return;
            }
            Console.WriteLine($"Received {amount} of bytes");
            string stream = Encoding.UTF8.GetString(buffer);
            Console.WriteLine(stream);
        }
    }

    public static void OnConnect(EndPoint? clientRemoteEndPoint){
        Console.WriteLine($"test");
        Console.WriteLine($"Connected to: {clientRemoteEndPoint}");
    }
}
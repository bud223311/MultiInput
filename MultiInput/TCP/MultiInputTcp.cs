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
        private bool _connected;

        public void Awake(){
            _connected = false;
            Listener.Start();
            Key.InitializeKeys();
            
        }
        public void Update(){
            if (!Listener.Server.Connected) {
                _connected = false;
                return;
            }
            if (!_connected) {
                OnConnect(Listener.Server.RemoteEndPoint);
                _connected = true;
            }
            Key.UpdateKeys(this);
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
            


            var buffer = new Span<byte>();
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
        Console.WriteLine($"Connected to: {clientRemoteEndPoint}");
    }
}
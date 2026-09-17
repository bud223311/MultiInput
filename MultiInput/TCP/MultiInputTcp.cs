using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using Microsoft.VisualBasic;
using MultiInput.Enums;
using MultiInput.Enums.Constants;
using MultiInput.Inputter;
using MultiInput.Poller;

namespace MultiInput.TCP;

public class MultiInputTcp
{
    
    public class KTcpHost(TcpListener listener)
    {
        public TcpListener Listener = listener;
        public List<Socket?> Clients = new List<Socket?>();
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
                Socket? client = Listener.AcceptSocket();
                Clients.Add(client);
                PreConnect();
                OnConnect(client.RemoteEndPoint);
            }

            foreach (var client in Clients) {
                if (client is null||!client.Connected) {
                    if (_connected) {
                        OnDisconnect();
                    }
                    _connected = false;
                    return;
                }

                if (TcpPoll.TimeUntilPollAfterInput(30) is true) {
                    try {
                        Console.WriteLine($"Sending Ping to Client");
                        client.Send(Encoding.UTF8.GetBytes("ping"));
                    }
                    catch (Exception) {
                        OnDisconnect();
                    }
                }
                Key.UpdateKeys(client);
            }
        }

        public void Close(){
            foreach (var client in Clients) {
                if (client is not null) {
                    client.Close();
                }
            }
            Clients.Clear();
            Listener.Stop();
            StaticData.Host = null;
        }
    }

    public class KTcpClient(TcpClient client,IPAddress connectionTo,int port)
    {
        public TcpClient Client = client;
        public IPAddress ConnectionGoesTo = connectionTo;
        public int Port = port;
        private bool _lastconnectionstate;
        private KeyboardInput _keyboardInput = new KeyboardInput();
        public void Awake(){
            _lastconnectionstate = false;
            Console.WriteLine($"Connecting...");
            Client.Connect(ConnectionGoesTo, Port);
        }
        public void Update(){
            if (Client is{ Connected: false, Available: 0}) {
                if (_lastconnectionstate) {
                    OnDisconnect();
                }
                _lastconnectionstate = false;
                return;
            }

            if (!_lastconnectionstate) {
                OnConnect(Client.Client.RemoteEndPoint);
                _lastconnectionstate = true;
            }


            byte[] buffer = new byte[255];
            try {
                int amount = Client.Client.Receive(buffer);
                if (amount is 0) {
                    return;
                }
                Console.WriteLine($"Received {amount} of bytes");
                _keyboardInput.Handle(Encoding.UTF8.GetString(buffer),amount);
            }
            catch (Exception) {
                OnDisconnect();
            }
        }

        public void Close(){
            Client.Close();
            StaticData.Client = null;
        }
    }

    public static void PreConnect(){
        StaticData.TimesinceLastDataSent = DateTime.Now;
    }
    public static void OnConnect(EndPoint? clientRemoteEndPoint){
        Console.WriteLine($"Connected to: {clientRemoteEndPoint}");
    }

    public static void OnDisconnect(){
        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Disconnected");
    }
}
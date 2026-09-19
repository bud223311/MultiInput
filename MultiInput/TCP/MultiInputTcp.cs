using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using Microsoft.VisualBasic;
using MultiInput.Enums;
using MultiInput.Enums.Constants;
using MultiInput.Extensions;
using MultiInput.Inputter;
using MultiInput.Logging;
using MultiInput.Poller;

namespace MultiInput.TCP;

public class MultiInputTcp
{
    
    public class KTcpHost(TcpListener listener)
    {
        public TcpListener Listener = listener;
        private bool _lastconnectionstate;
        public static void InitializeHost(int port){
            StaticData.Host = new KTcpHost(TcpListener.Create(port));
        }

        public void Awake(){
            ConsoleLog.WriteConsoleMessage($"Waiting For Connections...", ConsoleColor.Cyan);
            _lastconnectionstate = false;
            Listener.Start();
            Key.InitializeKeys();
            
        }
        public void Update(){
            if (Listener.Pending()) {
                ConsoleLog.WriteConsoleMessage($"Found Pending Request",ConsoleColor.Green);
                Socket? client = Listener.AcceptSocket();
                StaticData.Clients.Add(client);
                PreConnect();
                OnConnect(client.RemoteEndPoint);
            }

            foreach (var client in StaticData.Clients) {
                if (client is null || !client.Connected) {
                    if (_lastconnectionstate) {
                        OnDisconnect();
                    }

                    _lastconnectionstate = false;
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
            }

            
            if (StaticData.InputType is InputType.Keyboard) {
                Key.UpdateKeys(StaticData.Clients);
            }
            else if (StaticData.InputType is InputType.Controller) {
                Mouse.UpdateMouse(StaticData.Clients);
            }
        }

        public void Close(){
            foreach (var client in StaticData.Clients) {
                if (client is not null) {
                    client.Close();
                }
            }
            StaticData.Clients.Clear();
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

        public static void InitializeClient(IPAddress address, int portresult){
            StaticData.Client = new KTcpClient(new TcpClient(),address,portresult);
        }
        public void Awake(){
            _lastconnectionstate = false;
            ConsoleLog.WriteConsoleMessage($"Connecting...", ConsoleColor.Yellow);
            try {
                Client.Connect(ConnectionGoesTo, Port);
            }
            catch (Exception e) {
                ConsoleLog.WriteConsoleMessage($"Failed to Connect: {e.Message}", ConsoleColor.Red);
                DebugLog.DebugMessageThread($"Failed to Connect: {e.Message}");
                OnDisconnect();
            }
        }
        public void Update(){
            if (Client is{ Connected: false, Available: 0}) {
                if (_lastconnectionstate) {
                    OnClientDisconnect();
                }
                _lastconnectionstate = false;
                return;
            }

            if (!_lastconnectionstate) {
                OnClientConnect(Client.Client.RemoteEndPoint);
                _lastconnectionstate = true;
            }

            DebugLog.WriteDebugMessage($"Client Connected: {Client.Connected} | Available: {Client.Available}");
            byte[] buffer = new byte[255];
            try {
                int amount = Client.Client.Receive(buffer);
                if (amount is 0) {
                    return;
                }
                DebugLog.WriteDebugMessage($"Received {amount} of bytes");
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
    public static void OnClientConnect(EndPoint? clientRemoteEndPoint){
        ConsoleLog.WriteConsoleMessage($"Connected to: {clientRemoteEndPoint}", ConsoleColor.Green);
    }
    public static void OnConnect(EndPoint? clientEndPoint){
        ConsoleLog.WriteConsoleMessage($"Client Connected: {clientEndPoint}", ConsoleColor.Green);
    }

    public static void OnClientDisconnect(){
        StaticData.Clients.RemoveNulls();
        ConsoleLog.WriteOnlyColoredTimeStamp($"Disconnected | Clients Connected: {StaticData.Clients.Count}", ConsoleColor.Red);
        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Disconnected");
    }
    public static void OnDisconnect(){
        StaticData.WasDisconnected = true;
        if (StaticData.Client is not null) {
            StaticData.Client.Close();
        }
    }
}
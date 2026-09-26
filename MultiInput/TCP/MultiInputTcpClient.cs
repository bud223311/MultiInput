using System.Net;
using System.Net.Sockets;
using System.Text;
using MultiInput.Inputter;
using MultiInput.Logging;

namespace MultiInput.TCP;

public partial class MultiInputTcp
{
    public class MultiInputTcpClient(TcpClient client,IPAddress connectionTo,int port)
    {
        private Configuration config;
        public TcpClient Client = client;
        public IPAddress ConnectionGoesTo = connectionTo;
        public int Port = port;
        private bool _lastconnectionstate;
        private KeyboardInput _keyboardInput = new KeyboardInput();

        public static void InitializeClient(IPAddress address, int portresult){
            StaticData.Client = new MultiInputTcpClient(new TcpClient(),address,portresult);
        }
        public void Awake(){
            config = new Configuration();
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
                    OnDisconnect();
                }
                _lastconnectionstate = false;
                return;
            }

            if (!_lastconnectionstate) {
                OnConnect(Client.Client.RemoteEndPoint);
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
            catch (Exception e) {
                ConsoleLog.WriteConsoleMessage($"An Error occured in TCP Client {e}");
                DebugLog.WriteDebugMessage($"An Error occured in TCP Client {e}");
                OnDisconnect();
            }
        }

        public void OnConnect(EndPoint? clientRemoteEndPoint){
            ConsoleLog.WriteConsoleMessage($"Connected to: {clientRemoteEndPoint}", ConsoleColor.Green);
        }

        public void OnDisconnect(){
            KeyboardInput.ReleaseAll();
        
            StaticData.WasDisconnected = true;
        
            StaticData.Client?.Close();
        }

        public void Close(){
            Client.Close();
            StaticData.Client = null;
        }
    }
}
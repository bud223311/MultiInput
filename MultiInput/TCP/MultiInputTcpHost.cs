using System.Net;
using System.Net.Sockets;
using System.Text;
using MultiInput.Extensions;
using MultiInput.Inputter;
using MultiInput.Logging;
using MultiInput.Poller;

namespace MultiInput.TCP;

public partial class MultiInputTcp
{
    public class MultiInputTcpHost(TcpListener listener)
    {
        public TcpListener Listener = listener;
        private bool _lastconnectionstate;
        public static void InitializeHost(int port){
            StaticData.Host = new MultiInputTcpHost(TcpListener.Create(port));
        }

        public void Awake(){
            ConsoleLog.WriteConsoleMessage($"Waiting For Connections...", ConsoleColor.Cyan);
            _lastconnectionstate = false;
            Listener.Start();
            if (StaticData.InputType is InputType.Keyboard) {
                Key.InitializeKeys();
            }

            if (StaticData.InputType is InputType.Controller) {
                ControllerReader.XInput.InitializeButtons();
            }
            
        }
        public void Update(){
            if (Listener.Pending()) {
                ConsoleLog.WriteConsoleMessage($"Found Pending Request",ConsoleColor.Green);
                if (!PreConnect()) {
                    return;
                }
                Socket? client = Listener.AcceptSocket();
                StaticData.Clients.Add(client);
                
                OnConnect(client.RemoteEndPoint);
            }

            if (StaticData.Clients.Count == 0) {
                return;
            }
            /*
             * List<Socket> disconnectedSockets = new List<Socket>();
                foreach (var client in StaticData.Clients) {
                if (!client.Connected) {
                    disconnectedSockets.Add(client);
                }
            }
             */

            var disconnectedSockets = StaticData.Clients.Where(client => !client.Connected).ToList();
            //92.17.121.207
            foreach (var disconnectedSocket in disconnectedSockets) {
                OnDisconnect(disconnectedSocket);
            }
            disconnectedSockets.Clear();
            
            if (TcpPoll.TimeUntilPollAfterInput(30) is true) {
                TrySendToAll(Encoding.UTF8.GetBytes($"ping"));
            }

            
            switch (StaticData.InputType) {
                case InputType.Keyboard:
                    Key.UpdateKeys(StaticData.Clients);
                    break;
                case InputType.Controller:
                {
                    int id = ControllerReader.XInput.GetFirstConnectedController();
                    ControllerReader.TargettedControllerIndex = id;
                    if (id is -1) {
                        DebugLog.DebugMessageThread($"No Controller Connected");
                        return;
                    }
                    ControllerReader.XInput.ReadAllButtons();
                    break;
                }
                default:
                    ConsoleLog.WriteConsoleMessage($"Error InputType is invalid ~~ {StaticData.InputType}",ConsoleColor.Red);
                    DebugLog.WriteDebugMessage($"Error InputType is invalid ~~ {StaticData.InputType}");
                    break;
            }
        }

        public bool PreConnect(){
            StaticData.Clients.RemoveDisconnected();
            StaticData.TimesinceLastDataSent = DateTime.Now;
            return true;
        }

        public void OnConnect(EndPoint? endPoint){
            
        }

        public bool TrySendToAll(byte[] data){
            bool flag = false;
            foreach (var client in StaticData.Clients) {
                try {
                    client.Send(data);
                    flag = true;
                }
                catch (Exception e) {
                    DebugLog.WriteDebugMessage($"Failed To Send Data to Client {client.RemoteEndPoint} {e.Message}");
                    ConsoleLog.WriteConsoleMessage($"Failed To Send Data to Client {client.RemoteEndPoint} {e.Message}");
                }
            }

            return flag;
        }

        public void OnDisconnect(Socket disconnectedSocket){
            StaticData.Clients.Remove(disconnectedSocket);
            ConsoleLog.WriteConsoleMessage($"Disconnected {disconnectedSocket.RemoteEndPoint} | Clients Connected: {StaticData.Clients.Count}", ConsoleColor.Yellow);
        }

        public void Close(){
            foreach (var client in StaticData.Clients) {
                client.Close();
            }
            StaticData.Clients.Clear();
            Listener.Stop();
            StaticData.Host = null;
        }
    }
}
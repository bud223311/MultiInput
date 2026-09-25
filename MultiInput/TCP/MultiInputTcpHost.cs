using System.Net;
using System.Net.Sockets;
using System.Text;
using MultiInput.ControllerUtils;
using MultiInput.Extensions;
using MultiInput.Inputter;
using MultiInput.Logging;
using MultiInput.Poller;
using MultiInput.Timer;

namespace MultiInput.TCP;

public partial class MultiInputTcp
{
    public class MultiInputTcpHost(TcpListener listener)
    {
        public TcpListener Listener = listener;
        private bool _restrictLogSpamming;
        private bool _hasvibratedController;
        public static void InitializeHost(int port){
            StaticData.Host = new MultiInputTcpHost(TcpListener.Create(port));
        }

        public void Awake(){
            Configuration.ReadConfig();
            ConsoleLog.WriteConsoleMessage($"Waiting For Connections...", ConsoleColor.Cyan);
            Listener.Start();
            switch (StaticData.InputType) {
                case InputType.Keyboard:
                    KeyboardReader.InitializeKeys();
                    break;
                case InputType.Controller:
                    if (ControllerReader.TryGetDualShockController()) {
                        break;
                    }
                    ControllerReader.XInput.InitializeButtons();
                    break;
            }
        }
        public void Update(){
            if (Listener.Pending()) {
                ConsoleLog.WriteConsoleMessage($"Found Pending Request",ConsoleColor.Green);
                if (!PreConnect()) {
                    return;
                }
                Socket client = Listener.AcceptSocket();
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
                    KeyboardReader.UpdateKeys(StaticData.Clients);
                    break;
                case InputType.Controller:
                {
                    if (StaticData.ControllerInputType == ControllerInputType.DualSense) {
                        break;
                    }
                    int id = ControllerReader.XInput.GetFirstConnectedController();
                    ControllerReader.TargettedControllerIndex = id;
                    if (id is -1) {
                        if (_restrictLogSpamming) {
                            return;
                        }

                        _hasvibratedController = false;
                        _restrictLogSpamming = true;
                        ConsoleLog.WriteConsoleMessage($"No Controller Connected");
                        DebugLog.DebugMessageThread($"No Controller Connected");
                        return;
                    }

                    if (!_hasvibratedController) {
                        _hasvibratedController = true;
                        ControllerUtilities.CreateThreadedControllerVibrationStartup(ControllerInputType.XInput);
                    }

                    _restrictLogSpamming = false;
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
            ConsoleLog.WriteConsoleMessage($"{endPoint} Connected");
        }
        
        /// <summary>
        /// Returns True if Any Data was sent
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool TrySendToAll(byte[] data){
            bool flag = false;
            foreach (var client in StaticData.Clients) {
                try {
                    client.Send(data);
                    flag = true;
                    StaticData.TimesinceLastDataSent = DateTime.Now;
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
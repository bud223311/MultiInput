using MultiInput.Configuration;
using MultiInput.ControllerUtils;
using MultiInput.Extensions;
using MultiInput.Inputter;
using MultiInput.Inputter.Controller;
using MultiInput.Logging;
using MultiInput.Poller;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MultiInput.TCP;

public partial class MultiInputTcp
{
    public class MultiInputTcpHost(TcpListener listener)
    {
        private ConfigurationFile config;
        private IInputReader reader;
        public TcpListener Listener = listener;
        public static void InitializeHost(int port){
            StaticData.Host = new MultiInputTcpHost(TcpListener.Create(port));
        }
        public void OnCloseProgram()
        {
            reader.OnCloseProgram();
        }

        public void Awake(){
            config = new ConfigurationFile();
            ConsoleLog.WriteConsoleMessage($"Waiting For Connections...", ConsoleColor.Cyan);
            Listener.Start();
            switch (StaticData.InputType) {
                case InputType.Keyboard:
                    reader = new KeyboardReader();
                    break;
                case InputType.Controller:
                    reader = new ControllerReader();
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

            var disconnectedSockets = StaticData.Clients.Where(client => !client.Connected).ToList();
            
            foreach (var disconnectedSocket in disconnectedSockets) {
                OnDisconnect(disconnectedSocket);
            }
            disconnectedSockets.Clear();
            
            if (TcpPoll.TimeUntilPollAfterInput(30) is true) {
                TrySendToAll(Encoding.UTF8.GetBytes($"ping"));
            }

            reader.UpdateBindings(StaticData.Clients);
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
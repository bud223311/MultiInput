using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
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

        public void Awake(){
            Listener.Start();
            Key.InitializeKeys();
            
        }
        public void Update(){
            if (!Listener.Server.Connected) {
                return;
            }
            Key.UpdateKeys(this);
        }
    }

    public class KTcpClient(TcpClient client,IPAddress connectionTo,int port)
    {
        public TcpClient Client = client;
        public IPAddress ConnectionGoesTo = connectionTo;
        public int Port = port;
        
        public void Awake(){
            Client.Connect(ConnectionGoesTo, Port);
        }
        public void Update(){
            if (Client is{ Connected: false, Available: 0}) {
                return;
            }   

            var buffer = new byte[8];
            
            Client.Client.Receive(buffer);
            if (buffer[0] != 8) return;
            KeyboardInput.Handle(buffer[1]);
        }
    }
}
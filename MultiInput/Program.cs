using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp1;

internal class Server
{
    //[DllImport("user32.dll")]
    //static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    internal const bool Debug = true;
    private static uint hwnd = 0;
    private static string RunTime = string.Format("{0:D}", System.DateTime.Now);
    private static readonly string LogPath = Path.Combine(AppContext.BaseDirectory, $"log {RunTime.Replace(':', '.')}.txt");

    //Launch args validation and handling
    public static void Main(string[] args)
    {
        Console.WriteLine(LogPath);

        if (args.Length == 0)
        {
            Console.WriteLine("No arguments found...");
            throw new Exception("No arguments provided. Please use the attached executable.");
        }

        hwnd = uint.Parse(args[0]);
        string LaunchType = args[1];
        string IP = args[2];
        int Port = int.Parse(args[3]);

        if (!Regex.IsMatch(hwnd.ToString(), "[0-9]{6,7}"))
        {
            Console.WriteLine($"HWND ({hwnd}) specified, connecting to AHK...");
        }
        //add hwnd validity check unless in debug



        switch (LaunchType)
        {
            case "receiver":
                ServerListener(Port);
                break;
            case "sender":
                ServerSender(IP, Port);
                break;
        }
    }


    //Start TCP client (Receiver)
    public static void ServerListener(int Port)
    {
        TcpListener server = new(IPAddress.Any, Port);

        FileStream fs = new(LogPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, bufferSize: 8);
        {
            //Get newline byte value for formatting log
            var newlinetxt = Environment.NewLine;
            byte[] newline = Encoding.UTF8.GetBytes(newlinetxt);

            //Position file pointer to new line at file end
            fs.Position = fs.Seek(0, SeekOrigin.End);
            fs.WriteAsync(newline, 0, newlinetxt.Length);

            server.Start();
            Console.WriteLine($"Waiting for connection on port: {Port}");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                byte[] message = new byte[data.Length];
                for (int i = 0; i < message.Length; i++)
                {
                    message[i] = buffer[i];
                }
                Console.WriteLine($"Received: {data}");

                //write newline before each new log entry
                fs.Write(newline, 0, newlinetxt.Length);
                fs.Write(message, 0, message.Length);

                Console.WriteLine($"{buffer[0]}");

                client.Close();
            }
        }
    }

    //Start TCP server (Sender)
    private static void ServerSender(string IP, int Port)
    {
        FileStream fs = new(LogPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize: 8);
        {
            var newlinetxt = Environment.NewLine;
            byte[] newline = Encoding.UTF8.GetBytes(newlinetxt);

            fs.Position = fs.Seek(0, SeekOrigin.End);
            fs.WriteAsync(newline, 0, newlinetxt.Length);
            //debug

            Console.WriteLine("Ready to send data.");
            while (true)
            {
                //add read logpath stream for getting data from ahk

                TcpClient client = new(IP, Port);
                NetworkStream stream = client.GetStream();


                int key = fs.ReadByte();



                byte[] data = Encoding.UTF8.GetBytes(key.ToString());
                stream.Write(data, 0, data.Length);

                client.Close();
            }
        }
    }






}
/*
 TODO:
    Send inputs from ahk to program
    Log sent/received inputs
    Make ahk script read logs data for inputs
    Validate IP formatting & connectivity
    Verify HWND to be ahk script
    Connection authentication for recipient
    Connection validation for sender + receiver
    Store historical IP + Ports
    Config to hide plaintext IP's
    Configurable rate limit
    GUI clean-up & feature additions
    Hotkeys for enable/disable (+ quick exit)
    
 */
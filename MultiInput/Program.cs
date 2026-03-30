using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
namespace ConsoleApp1;

internal class Server
{
    [DllImport("user32.dll")]
    static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    internal const bool Debug = true;
    private static bool NoStartup = false;
    private static uint hwnd = 0;

    public static void Main(string[] args)
    {
       hwnd = uint.Parse(args[0]);
        
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments found...");
        }
        else
        {
            if (Regex.IsMatch(args[0], "[0-9]{6,7}"))
            {
                Console.WriteLine("HWND (" + hwnd + ") specified, connecting to AHK...");
            }
        }
        try
        {
            switch (args[1])
            {
                case "receiver":
                    ServerListener();
                    break;
                case "sender":
                    if (Regex.IsMatch(args[3], "[0-9]{0,5}"))
                    {
                        ServerSender(args[2], args[3]);
                    }
                    
                    break;
            }
        }
        catch
        {
            Console.WriteLine("This application must be ran using the GUI utility that is included.");
        }
    }








    //Start TCP server listener
    public static void ServerListener()
    {
        TcpListener server = new(IPAddress.Any, 6567);
        server.Start();
        Console.WriteLine("Listening...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received: " + data);
            Recieved(data, client, stream);
        }
    }

    //Handle received TCP data from server
    private static void Recieved(string data, TcpClient client, NetworkStream stream)
    {
        if (data.Length != 5)
        {
            client.Close();
            return;
        }
        //PostMessage(hwnd, data,0,0);
        
        /*
        if (Debug)
        {
            if (NoStartup)
            {
                //Broken LogFile($"{data}     RequestFrom: {client.Client.RemoteEndPoint}");
                return;
            }

            //Broken  LogFile($"{data} {StartupArguments[0]}    RequestFrom: {client.Client.RemoteEndPoint}");
            //SendDataToAhk(HWND, data);
        }
        */
    }


    //Handle sending TCP data to client
    public static void ServerSender(string ip, string port)
    {
        Console.WriteLine("Ready to send data.");
        while (true)
        {
            string key = "Console.ReadLine()";
            Console.WriteLine(key);

            TcpClient client = new TcpClient(ip, int.Parse(port));
            NetworkStream stream = client.GetStream();

            byte[] data = Encoding.UTF8.GetBytes(key);
            stream.Write(data, 0, data.Length);

            client.Close();
        }
    }

    //Log file handling
    private static void LogFile(string data)
    {
        if (OperatingSystem.IsWindows())
        {
            var wUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            try
            {
                var regexArray = Regex.Split(wUser, @"^(.*?)\\");
                wUser = regexArray[2];
            }
            catch (Exception) { throw; }


            var path = $@"C:\Users\{wUser}\Documents\KiplingStuff";
            var LogPath = $@"C:\Users\{wUser}\Documents\KiplingStuff\Log";

            if (!File.Exists($"{LogPath}\\ServerLog.txt"))
            {
                Directory.CreateDirectory(LogPath);
                File.Create($"{LogPath}\\ServerLog.txt");
            }
            File.WriteAllText(LogPath, data);
        }


    }
}

using System.Text.RegularExpressions;
using MultiInput.TCP;

namespace MultiInput;

internal class TcpServer
{
    //[DllImport("user32.dll")]
    //static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    internal const bool Debug = true;
    private static uint _hwnd;
    private static readonly string RunTime = $"{DateTime.Now:D}";
    internal static readonly string LogPath = Path.Combine(AppContext.BaseDirectory, $"log {RunTime.Replace(':', '.')}.txt");

    //Launch args validation and handling
    public static void Main(string[] args)
    {
        Console.WriteLine(LogPath);

        if (args.Length == 0)
        {
            Console.WriteLine("No arguments found...");
            throw new Exception("No arguments provided. Please use the attached executable.");
        }
        _hwnd = uint.Parse(args[0]);
        string launchType = args[1];
        string ip = args[2];
        int port = int.Parse(args[3]);

        if (!Regex.IsMatch(_hwnd.ToString(), "[0-9]{6,7}"))
        {
            Console.WriteLine($"HWND ({_hwnd}) specified, connecting to AHK...");
        }
        //add hwnd validity check unless in debug

        switch (launchType)
        {
            case "receiver":
                //Start TCP client (Receiver)
                TcpReceiver.ServerListener(port);
                break;
            case "sender":
                //Start TCP server (Sender)
                TcpSender.ServerSender(ip, port);
                break;
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
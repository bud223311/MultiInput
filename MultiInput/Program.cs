using System.Net;
using System.Runtime.InteropServices;
using MultiInput.Extensions;
using MultiInput.Inputter;
using MultiInput.Logging;
using MultiInput.TCP;

namespace MultiInput;

internal static class TcpServer
{
    public static readonly Version ProgramVersion = new (0,0,1);
    private static readonly string RunTime = $"{DateTime.Now}";
    public static bool EndProgram;

    //Launch args validation and handling
    public static void Main(string[] args){
        OnStartup();
        
        ConsoleLog.WriteConsoleMessage($"MultiInput TCP Server v1.0.0\nStarted at: {RunTime}\nDebug Log Path: {DebugLog.GetPathOfDebugLog()}", ConsoleColor.Cyan);
        
        while (!EndProgram) {
            ConsoleLog.WriteConsoleMessage($"Connect to a machine running this application using: ip:port e.g (127.0.0.1:7777)\nOr start accepting Connections using accept or a", ConsoleColor.Green);
            string consoleStr = Console.ReadLine() ?? string.Empty;
            
            DebugLog.DebugMessageThread($"Console Input: {consoleStr}");
            switch(consoleStr.ToLowerInvariant())
            {
                case "exit":
                case "e":
                case "quit":
                case "q":
                    {
                        EndProgram = true;
                        continue;
                    }
                case "accept":
                case "a":
                case "acc":
                    {
                        StartAcceptConnections();
                        continue;
                    }
                case "":
                    {
                        ConsoleLog.WriteConsoleMessage($"No Input Provided", ConsoleColor.Red);
                        continue;
                    }
                default:
                    {
                        AttemptConnection(consoleStr);
                        continue;
                    }
            }
        }
        
        OnCloseProgram(1);
    }
    private static void AttemptConnection(string input)
    {
        if (!TryGetIPPort(input, out IPAddress? address, out int portresult))
        {
            ConsoleLog.WriteConsoleMessage($"Failed to Parse IP:Port: {input}", ConsoleColor.Red);
            DebugLog.DebugMessageThread($"Failed to Parse IP:Port: {input}");
            return;
        }

        DebugLog.DebugMessageThread($"Connecting to {address}:{portresult}");
        MultiInputTcp.MultiInputTcpClient.InitializeClient(address, portresult);

        if (StaticData.Client is null)
        {
            ConsoleLog.WriteConsoleMessage($"Failed to Initialize Client: {input}", ConsoleColor.Red);
            DebugLog.DebugMessageThread($"Failed to Initialize Client: {input}");
            return;
        }


        StaticData.Client.Awake();
        StaticData.WasDisconnected = false;

        while (!StaticData.WasDisconnected)
        {
            if (StaticData.Client is null)
            {
                StaticData.WasDisconnected = true;
            }
            StaticData.Client?.Update();
        }

        DebugLog.DebugMessageThread($"Disconnected from {address}:{portresult}");
        StaticData.Client?.Close();
    }
    private static void StartAcceptConnections()
    {
        ConsoleLog.WriteConsoleMessage($"Port in which to accept on?", ConsoleColor.Yellow);
        string port = Console.ReadLine() ?? string.Empty;

        if (!int.TryParse(port, out int tPort))
        {
            ConsoleLog.WriteConsoleMessage($"Invalid value for a port address: {port}", ConsoleColor.Red);
            return;
        }

        DebugLog.DebugMessageThread($"Accepting Connections on Port: {tPort}");
        MultiInputTcp.MultiInputTcpHost.InitializeHost(tPort);
        if (StaticData.Host is null)
        {
            ConsoleLog.WriteConsoleMessage($"Failed to Initialize Host: {tPort}", ConsoleColor.Red);
            DebugLog.DebugMessageThread($"Failed to Initialize Host: {tPort}");
            return;
        }
        ConsoleLog.WriteConsoleMessage($"InputMethod:\n0. Keyboard\n1. Controller");
        if (!InputType.TryParse(Console.ReadLine(), out InputType inputMethod))
        {
            ConsoleLog.WriteConsoleMessage($"InputMethod TryParse Failed", ConsoleColor.Red);
            DebugLog.DebugMessageThread($"InputMethod TryParse Failed");
            return;
        }
        StaticData.InputType = inputMethod;

        StaticData.Host.Awake();
        StaticData.WasDisconnected = false;

        while (!StaticData.WasDisconnected)
        {
            StaticData.Host.Update();
        }

        DebugLog.DebugMessageThread($"Stopped Accepting Connections on Port: {tPort}");
        StaticData.Host.Close();
    }
    private static bool TryGetIPPort(string input, out IPAddress? address, out int portresult)
    {
        address = default;
        portresult = -1;

        if (!input.ValidateIpPort())
        {
            ConsoleLog.WriteConsoleMessage($"Validation Failed: {input}", ConsoleColor.Red);
            DebugLog.DebugMessageThread($"Validation Failed: {input}");
            return false;
        }
        if (!IPAddress.TryParse(input.Split(':')[0], out address))
        {
            ConsoleLog.WriteConsoleMessage($"IPAddress TryParse Failed: {input}", ConsoleColor.Red);
            DebugLog.DebugMessageThread($"IPAddress TryParse Failed: {input}");
            return false;
        }
        if (!int.TryParse(input.Split(':')[1], out portresult))
        {
            ConsoleLog.WriteConsoleMessage($"Port TryParse Failed: {input}", ConsoleColor.Red);
            DebugLog.DebugMessageThread($"Port TryParse Failed: {input}");
            return false;
        }
        DebugLog.DebugMessageThread($"Parsed IP: {address}, Port: {portresult}");
        return true;    
    }

    private static bool OnCloseProgram(int eventType) {
        ConsoleLog.WriteConsoleMessage($"Closing Program...", ConsoleColor.Yellow);
        KeyboardInput.ReleaseAll();
        DebugLog.CreatePreviousDebugLog();
        ConsoleLog.WriteConsoleMessage($"Program Closed at: {DateTime.Now}", ConsoleColor.Green);
        StaticData.DualSense?.EndPolling();
        StaticData.DualSense?.Release();
        return false;
    }

    private static ConsoleEventDelegate _handler = null!;   
    // Pinvoke
    private delegate bool ConsoleEventDelegate(int eventType);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    
    public static void InitializeHandler(){
        _handler = OnCloseProgram;
        SetConsoleCtrlHandler(_handler, true);
    }

    public static void OnStartup(){
        InitializeHandler();
        DebugLog.InitializeStartup();
    }
}
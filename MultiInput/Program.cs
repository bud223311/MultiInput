using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MultiInput.Extensions;
using MultiInput.Logging;
using MultiInput.TCP;

namespace MultiInput;

internal static class TcpServer
{
    //16670
    
    private static readonly string RunTime = $"{DateTime.Now}";
    public static bool EndProgram = false;

    //Launch args validation and handling
    public static void Main(string[] args){
        handler = new ConsoleEventDelegate(OnCloseProgram);
        SetConsoleCtrlHandler(handler, true);
        DebugLog.InitializeStartup();
        ConsoleLog.WriteConsoleMessage($"MultiInput TCP Server v1.0.0\nStarted at: {RunTime}\nDebug Log Path: {DebugLog.GetPathOfDebugLog()}", ConsoleColor.Cyan);
        while (!EndProgram) {
            ConsoleLog.WriteConsoleMessage($"Connect to a machine running this application using: ip:port e.g (127.0.0.1:7777)\nOr start accepting Connections using accept or a", ConsoleColor.Green);
            string consoleStr = Console.ReadLine() ?? string.Empty;
            DebugLog.DebugMessageThread($"Console Input: {consoleStr}");
            if (consoleStr.ToLowerInvariant() is "exit" or "e" or "quit" or "q") {
                EndProgram = true;
                continue;
            }
            
            if (consoleStr.ToLowerInvariant() is "accept" or "a" or "acc") {
                ConsoleLog.WriteConsoleMessage($"Port in which to accept on?",ConsoleColor.Yellow);
                string port = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(port,out int tPort)) {
                    continue;
                }
                DebugLog.DebugMessageThread($"Accepting Connections on Port: {tPort}");
                StaticData.Host = new MultiInputTcp.KTcpHost(TcpListener.Create(tPort));
                StaticData.Host.Awake();
                StaticData.WasDisconnected = false;
                while (!StaticData.WasDisconnected) {
                    StaticData.Host.Update();
                }
                DebugLog.DebugMessageThread($"Stopped Accepting Connections on Port: {tPort}");
                StaticData.Host.Close();
                continue;
            }

            if (consoleStr == string.Empty) {
                Console.WriteLine($"consoleStr is Empty");
                continue;
            }

            if (!consoleStr.ValidateIpPort()) {
                Console.WriteLine($"Validation Failed: {consoleStr}");
                continue;
            }

            if (!IPAddress.TryParse(consoleStr.Split(':')[0], out var address)) {
                Console.WriteLine($"IPAddress TryParse Failed: {consoleStr}");
                continue;
            }

            if (!int.TryParse(consoleStr.Split(':')[1], out int portresult)) {
                Console.WriteLine($"Port TryParse Failed: {consoleStr}");
                continue;
            }
            DebugLog.DebugMessageThread($"Connecting to {address}:{portresult}");
            StaticData.Client = new MultiInputTcp.KTcpClient(new TcpClient(),address,portresult);
            StaticData.Client.Awake();
            StaticData.WasDisconnected = false;
            while (!StaticData.WasDisconnected) {
                StaticData.Client.Update();
            }
            DebugLog.DebugMessageThread($"Disconnected from {address}:{portresult}");
            StaticData.Client.Close();
        }
        OnCloseProgram(1);
    }
    private static bool OnCloseProgram(int eventType) {
        ConsoleLog.WriteConsoleMessage($"Closing Program...", ConsoleColor.Red);
        DebugLog.CreatePreviousDebugLog();
        return false;
    }
    
    static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
    // Pinvoke
    private delegate bool ConsoleEventDelegate(int eventType);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
}
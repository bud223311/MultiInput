using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using MultiInput.Extensions;
using MultiInput.TCP;

namespace MultiInput;

internal static class TcpServer
{
    //16670
    
    private static readonly string RunTime = $"{DateTime.Now}";
    public static bool EndProgram = false;

    //Launch args validation and handling
    public static void Main(string[] args){
        while (!EndProgram) {
            Console.WriteLine($"Connect to a machine running this application using:\nip:port e.g(127.0.0.1:7777)\nOr start accepting Connections using\naccept or a");
            string consoleStr = Console.ReadLine() ?? string.Empty;
            
            
            if (consoleStr.ToLowerInvariant() is "accept" or "a" or "acc") {
                Console.WriteLine($"Port in which to accept on?");
                string port = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(port,out int tPort)) {
                    continue;
                }
                
                StaticData.Host = new MultiInputTcp.KTcpHost(TcpListener.Create(tPort));
                StaticData.Host.Awake();
                StaticData.WasDisconnected = false;
                while (!StaticData.WasDisconnected) {
                    StaticData.Host.Update();
                }
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

            StaticData.Client = new MultiInputTcp.KTcpClient(new TcpClient(),address,portresult);
            StaticData.Client.Awake();
            StaticData.WasDisconnected = false;
            while (!StaticData.WasDisconnected) {
                StaticData.Client.Update();
            }
            StaticData.Client.Close();
        }
    }
}
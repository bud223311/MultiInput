using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using MultiInput.Extensions;
using MultiInput.TCP;

namespace MultiInput;

internal static class TcpServer
{
    
    private static readonly string RunTime = $"{DateTime.Now}";
    public static bool EndProgram = false;
    public static bool WriteInstruction = true;

    //Launch args validation and handling
    public static void Main(string[] args){
        while (!EndProgram) {
            if (WriteInstruction)
                Console.WriteLine($"Connect to a machine running this application using:\nip:port e.g(127.0.0.1:7777)\nOr start accepting Connections using\naccept or a");
            WriteInstruction = false;
            if (!WriteInstruction) {
                Console.WriteLine($"Error Occured :/");
            }
            string consoleStr = Console.ReadLine() ?? string.Empty;
            if (consoleStr.ToLowerInvariant() is "accept" or "a" or "acc") {
                Console.WriteLine($"Port in which to accept on?");
                string port = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(port,out int tPort)) {
                    continue;
                }

                MultiInputTcp.KTcpHost host = new MultiInputTcp.KTcpHost(TcpListener.Create(tPort));
                host.Awake();
                while (!EndProgram) {
                    host.Update();
                }
            }

            if (consoleStr == string.Empty) {
                continue;
            }

            if (!consoleStr.ValidateIpPort()) {
                continue;
            }

            if (!IPAddress.TryParse(consoleStr.Split(':')[0], out var address)) {
                continue;
            }

            if (!int.TryParse(consoleStr.Split(':')[1], out int portresult)) {
                continue;
            }

            var tcpClient = new MultiInputTcp.KTcpClient(new TcpClient(),address,portresult);
            tcpClient.Awake();
            while (!EndProgram) {
                tcpClient.Update();
            }
        }
    }
}
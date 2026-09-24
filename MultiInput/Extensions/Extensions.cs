using System.Net.Sockets;
using MultiInput.Logging;

namespace MultiInput.Extensions;

public static class StringExtensions
{
    public static bool ValidateIpPort(this string ip){
        string[] checkport = ip.Split(':');
        if (checkport.Length is not 2) {
            return false;
        }
        string[] checkip = checkport[0].Split('.');
        return checkip.Length is > 2 and < 6;
    }
}
public static class ListExtensions
{
    public static void RemoveNulls<T>(this List<T?> list) where T : class{
        list.RemoveAll(item => item is null);
    }

    public static void RemoveDisconnected(this List<Socket> list){
        int count = list.RemoveAll(x => !x.Connected);
        ConsoleLog.WriteConsoleMessage($"Removing {count} clients");
    }
}
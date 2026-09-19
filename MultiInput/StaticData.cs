using System.Net.Sockets;
using MultiInput.TCP;

namespace MultiInput;

public static class StaticData
{
    public static DateTime StartTime => DateTime.Now;
    public static DateTime? TimesinceLastDataSent;
    public static bool WasDisconnected = false;
    internal static bool ToggleInput = true;
    public static InputType InputType;
    public static MultiInputTcp.KTcpHost? Host;
    public static MultiInputTcp.KTcpClient? Client;
    public static List<Socket?> Clients = new List<Socket?>();
}

public enum InputType
{
    Keyboard = 0,
    Controller = 1,
}
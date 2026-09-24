using System.Net.Sockets;
using DualSenseAPI;
using MultiInput.TCP;

namespace MultiInput;

public static class StaticData
{
    public static DateTime StartTime => DateTime.Now;
    public static DateTime? TimesinceLastDataSent;
    public static bool WasDisconnected = false;
    internal static bool ToggleInput = true;
    internal static ControllerInputType ControllerInputType;
    internal static DualSense? DualSense;
    public static InputType InputType;
    public static MultiInputTcp.MultiInputTcpHost? Host;
    public static MultiInputTcp.MultiInputTcpClient? Client;
    public static List<Socket> Clients = new List<Socket>();
}

public enum InputType
{
    Keyboard = 0,
    Controller = 1,
}

public enum ControllerInputType
{
    XInput,
    DualSense
}

public class Data(InputType inputType, string key, int state)
{
    public InputType InputType = inputType;
    public string Key = key;
    public int State = state;
}
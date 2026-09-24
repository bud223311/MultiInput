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

public class Data
{
    public InputType InputType;
    public string Key;
    public int State;
    
    public Data(InputType inputType,string key,int state){
        InputType = inputType;
        Key = key;
        State = state;
    }
}
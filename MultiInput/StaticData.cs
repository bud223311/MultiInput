using DualSenseAPI;
using DualSenseAPI.State;
using MultiInput.Inputter.Controller.XInputController;
using MultiInput.TCP;
using System.Net.Sockets;

namespace MultiInput;

public static class StaticData
{
    public static DateTime StartTime => DateTime.Now;
    public static DateTime? TimesinceLastDataSent;
    public static bool WasDisconnected = false;
    internal static bool ToggleInput = true;
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
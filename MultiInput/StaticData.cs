using DualSenseAPI;
using DualSenseAPI.State;
using MultiInput.TCP;
using MultiInput.Timer;
using System.Net.Sockets;
using static MultiInput.Inputter.ControllerReader;

namespace MultiInput;

public static class StaticData
{
    public static DateTime StartTime => DateTime.Now;
    public static DateTime? TimesinceLastDataSent;
    public static bool WasDisconnected = false;
    internal static bool ToggleInput = true;
    internal static ControllerInputType ControllerInputType;
    internal static DualSense? DualSense
    {
        get
        {
            return DualSense;
        }
        set
        {
            if (value is null)
            {
                ControllerInputType = ControllerInputType.XInput;
                XInput.InitializeButtons();
                return;
            }
            DualSense = value;
            DualSense.Acquire();
            DualSense.OnButtonStateChanged += OnDualShockControllerStateChange;
            DualSense.BeginPolling(20);
            DualSense.OutputState.LeftRumble = 1f;
            DualSense.OutputState.RightRumble = 1f;
            WaitingTimer.WaitForMilliseconds(500);
            DualSense.OutputState.LeftRumble = 0f;
            DualSense.OutputState.RightRumble = 0f;
            ControllerInputType = ControllerInputType.DualSense;
        }
    }
    public static InputType InputType;
    public static MultiInputTcp.MultiInputTcpHost? Host;
    public static MultiInputTcp.MultiInputTcpClient? Client;
    public static List<Socket> Clients = new List<Socket>();
    private static void OnDualShockControllerStateChange(DualSense sender, DualSenseInputStateButtonDelta changes)
    {
        if (!changes.HasChanges) return;
        OnDualShockControllerButtonState(changes.DPadLeftButton, XInputButton.DPadLeft);
    }

    private static void OnDualShockControllerButtonState(ButtonDeltaState delta, XInputButton button)
    {
        switch (delta)
        {
            case ButtonDeltaState.Pressed:
                ButtonState.SendButtonPacket(InputType.Controller, button, held: true);
                break;
            case ButtonDeltaState.Released:
                ButtonState.SendButtonPacket(InputType.Controller, button, held: false);
                break;
        }
    }
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
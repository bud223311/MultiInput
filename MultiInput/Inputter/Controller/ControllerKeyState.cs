using System.Net.Sockets;
using System.Text;
using MultiInput.Enums.Constants;
using MultiInput.Logging;
using MultiInput.TCP;
using SDL3;
using WindowsInput.Native;

namespace MultiInput.Inputter.Controller;

public class ControllerKeyState
{

    public SDL.GamepadButton GamepadButton;
    public bool IsDown;
    
    public ControllerKeyState(SDL.GamepadButton gamepadButton)
    {
        GamepadButton = gamepadButton;
    }

    public void OnKeyStateChange(KeyStateChanged ev, List<Socket> clients)
    {
        DebugLog.DebugMessageThread($"KeyData | Key: {GamepadButton} | State: {ev}\nStaticData | InputLock:{StaticData.ToggleInput} | {string.Join(".", StaticData.Clients.Select(x => x.RemoteEndPoint))}");
        if (this.GamepadButton == SDL.GamepadButton.Misc1)
        {
            if (ev == KeyStateChanged.JustPressed)
            {
                StaticData.ToggleInput = !StaticData.ToggleInput;
                Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| TOGGLE INPUT: {StaticData.ToggleInput}");
            }
            return;
        }
        if (StaticData.Clients.Count is 0)
        {
            return;
        }
        if (!StaticData.ToggleInput)
        {
            return;
        }
        string data = $"{GamepadButton}.{ev.ToString("D")}";
        var span = Encoding.UTF8.GetBytes($"1.{data}");

        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| SEND {data}");
        DebugLog.DebugMessageThread($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| SEND {data}");
        MultiInputTcp.MultiInputTcpHost.TrySendToAll(span);
    }
}
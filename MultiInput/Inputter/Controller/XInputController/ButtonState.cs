using MultiInput.Extensions;
using MultiInput.Logging;
using MultiInput.TCP;

namespace MultiInput.Inputter.Controller.XInputController
{
    public class ButtonState
    {
        public XInputButton Button;
        public bool IsPressed;
        public bool WasPressedLastFrame;
        public ButtonState(XInputButton button)
        {
            Button = button;
            IsPressed = false;
        }

        public void NotifyButtonStateChange()
        {
            SendButtonPacket(InputType.Controller, Button, held: IsPressed);
        }

        public static void SendButtonPacket(InputType inputType, XInputButton button, bool held)
        {
            // if (StaticData.Clients.Count is 0) {
            //     return;
            // }
            DebugLog.DebugMessageThread($"KeyData | Key: {button} | State: {(held ? "On" : "Off")}\nStaticData | InputLock:{StaticData.ToggleInput} | {string.Join(".", StaticData.Clients.Select(x => x.RemoteEndPoint))}");

            if (!StaticData.ToggleInput)
            {
                return;
            }
            byte[] data = $"{(int)inputType}.{button}.{(held ? 1 : 0)}".ToBytes();

            MultiInputTcp.MultiInputTcpHost.TrySendToAll(data);
        }
    }
}

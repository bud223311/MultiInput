using MultiInput.Enums.Constants;
using MultiInput.Logging;
using MultiInput.TCP;
using System.Net.Sockets;
using System.Text;
using WindowsInput.Native;

namespace MultiInput.Inputter
{
    public class KeyState
    {
        public int VKey;
        public bool IsDown;

        public KeyState(int vKey)
        {
            VKey = vKey;
        }

        public void OnKeyStateChange(KeyStateChanged ev, List<Socket> clients)
        {
            DebugLog.DebugMessageThread($"KeyData | Key: {VKey} | State: {ev}\nStaticData | InputLock:{StaticData.ToggleInput} | {string.Join(".", StaticData.Clients.Select(x => x.RemoteEndPoint))}");
            if (this.VKey == (int)VirtualKeyCode.F3)
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
            string data = $"{VKey}.{ev.ToString("D")}";
            var span = Encoding.UTF8.GetBytes($"0.{data}");

            Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| SEND {data}");
            DebugLog.DebugMessageThread($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| SEND {data}");
            MultiInputTcp.MultiInputTcpHost.TrySendToAll(span);
        }
    }
}

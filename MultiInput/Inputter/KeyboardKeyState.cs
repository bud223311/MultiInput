using MultiInput.Enums.Constants;
using MultiInput.Logging;
using MultiInput.TCP;
using System.Net.Sockets;
using System.Text;
using MultiInput.Configuration;
using SDL3;
using WindowsInput.Native;

namespace MultiInput.Inputter
{
    public class KeyboardKeyState
    {
        public int VKey;
        public bool IsDown;

        public KeyboardKeyState(int vKey)
        {
            VKey = vKey;
        }

        public void OnKeyStateChange(KeyStateChanged ev, List<Socket> clients,ConfigurationFile config)
        {
            DebugLog.DebugMessageThread($"KeyData | Key: {VKey} | State: {ev}\nStaticData | InputLock:{StaticData.ToggleInput} | {string.Join(".", StaticData.Clients.Select(x => x.RemoteEndPoint))}");
            if (this.VKey == ConfigurationEntry.KeyboardToggleKey)
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
            ConfigurationEntry? configurationEntry = config.HostConfigs.FirstOrDefault(x =>
                x.ReplacingKey != null && x.ReplacingKey == VKey);

            if (configurationEntry is null) {
                DebugLog.DebugMessageThread($"No configuration found for Keyboard button: {VKey}");
                return;
            }


            string data = $"{configurationEntry.ReplacedKey}.{ev.ToString("D")}";
            var span = Encoding.UTF8.GetBytes($"0.{data}");

            Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| SEND {data}");
            DebugLog.DebugMessageThread($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| SEND {data}");
            MultiInputTcp.MultiInputTcpHost.TrySendToAll(span);
        }
    }
}

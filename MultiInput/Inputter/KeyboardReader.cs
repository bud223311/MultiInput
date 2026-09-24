using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using MultiInput.Enums.Constants;
using MultiInput.Logging;
using WindowsInput.Native;

namespace MultiInput.Inputter;

public class KeyboardReader
{
    public static Dictionary<int, KeyboardReader> Keys = new Dictionary<int, KeyboardReader>();
    public int VKey;
    public bool IsDown;

    public static void UpdateKeys(List<Socket> clients){
        foreach (var key in Keys.Values) {
            
            bool newState = IsKeyDown(key.VKey);
            if (key.IsDown != newState) {
                
                key.OnKeyStateChange(newState ? KeyStateChanged.JustPressed : KeyStateChanged.JustReleased,clients);
            }
            key.IsDown = newState;
        }
    }

    public void OnKeyStateChange(KeyStateChanged ev, List<Socket> clients){
        DebugLog.DebugMessageThread($"KeyData | Key: {VKey} | State: {ev}\nStaticData | InputLock:{StaticData.ToggleInput} | {string.Join(".",StaticData.Clients.Select(x=>x?.RemoteEndPoint))}");
        if (this.VKey == (int)VirtualKeyCode.F3) {
            if (ev == KeyStateChanged.JustPressed) {
                StaticData.ToggleInput = !StaticData.ToggleInput;
                Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| TOGGLE INPUT: {StaticData.ToggleInput}");
            }
            return;
        }
        if (StaticData.Clients.Count is 0) {
            return;
        }
        if (!StaticData.ToggleInput) {
            return;
        }
        
        string data = ev == KeyStateChanged.JustPressed ? $"{VKey}.1" : $"{VKey}.0";

        var span = Encoding.UTF8.GetBytes($"0.{data}");

        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| SEND {data}");
        DebugLog.DebugMessageThread($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| SEND {data}");
        try {
            DebugLog.DebugMessageThread($"Attempting to send data to {StaticData.Clients.Count} clients.");
            foreach (var client in StaticData.Clients) {
                client?.Send(span);
            }
            StaticData.TimesinceLastDataSent = DateTime.Now;
        }
        catch (Exception) {
            Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed to send data to remote host. ");
            DebugLog.DebugMessageThread($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed to send data to remote host. ");
        }
    }

    public static KeyboardReader? GetKeyByVKey(int key){
        return Keys.FirstOrDefault(x=>x.Key == key).Value;
    }
    public static void InitializeKeys(){
        new KeyboardReader((int)VirtualKeyCode.VK_A).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_B).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_C).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_D).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_E).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_F).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_G).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_H).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_I).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_J).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_K).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_L).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_M).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_N).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_O).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_P).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_Q).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_R).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_S).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_T).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_U).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_V).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_W).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_X).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_Y).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_Z).Register();
        
        new KeyboardReader((int)VirtualKeyCode.VK_0).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_1).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_2).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_3).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_4).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_5).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_6).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_7).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_8).Register();
        new KeyboardReader((int)VirtualKeyCode.VK_9).Register();
        
        new KeyboardReader((int)VirtualKeyCode.SPACE).Register();
        new KeyboardReader((int)VirtualKeyCode.MENU).Register();
        new KeyboardReader((int)VirtualKeyCode.ESCAPE).Register();
        new KeyboardReader((int)VirtualKeyCode.BACK).Register();
        new KeyboardReader((int)VirtualKeyCode.RETURN).Register();
        new KeyboardReader((int)VirtualKeyCode.LEFT).Register();
        new KeyboardReader((int)VirtualKeyCode.RIGHT).Register();
        new KeyboardReader((int)VirtualKeyCode.UP).Register();
        new KeyboardReader((int)VirtualKeyCode.DOWN).Register();

        //TOGGLE
        new KeyboardReader((int)VirtualKeyCode.F3).Register();
    }

    public KeyboardReader(int vKey){
        VKey = vKey;
    }

    public void Register(){
        Keys.Add(this.VKey,this);
    }
    [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    public static extern short GetKeyState(int nVirtKey);

    public static bool IsKeyDown(int nVirtKey){
        return GetKeyState(nVirtKey) < 0;
    }
    
}
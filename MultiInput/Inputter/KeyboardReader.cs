using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using MultiInput.Enums.Constants;
using MultiInput.Logging;
using MultiInput.TCP;
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
        DebugLog.DebugMessageThread($"KeyData | Key: {VKey} | State: {ev}\nStaticData | InputLock:{StaticData.ToggleInput} | {string.Join(".",StaticData.Clients.Select(x=>x.RemoteEndPoint))}");
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
        MultiInputTcp.MultiInputTcpHost.TrySendToAll(span);
    }

    public static KeyboardReader? GetKeyByVKey(int key){
        return Keys.FirstOrDefault(x=>x.Key == key).Value;
    }
    public static void InitializeKeys(){
        new KeyboardReader((int)VirtualKeyCode.VK_A);
        new KeyboardReader((int)VirtualKeyCode.VK_B);
        new KeyboardReader((int)VirtualKeyCode.VK_C);
        new KeyboardReader((int)VirtualKeyCode.VK_D);
        new KeyboardReader((int)VirtualKeyCode.VK_E);
        new KeyboardReader((int)VirtualKeyCode.VK_F);
        new KeyboardReader((int)VirtualKeyCode.VK_G);
        new KeyboardReader((int)VirtualKeyCode.VK_H);
        new KeyboardReader((int)VirtualKeyCode.VK_I);
        new KeyboardReader((int)VirtualKeyCode.VK_J);
        new KeyboardReader((int)VirtualKeyCode.VK_K);
        new KeyboardReader((int)VirtualKeyCode.VK_L);
        new KeyboardReader((int)VirtualKeyCode.VK_M);
        new KeyboardReader((int)VirtualKeyCode.VK_N);
        new KeyboardReader((int)VirtualKeyCode.VK_O);
        new KeyboardReader((int)VirtualKeyCode.VK_P);
        new KeyboardReader((int)VirtualKeyCode.VK_Q);
        new KeyboardReader((int)VirtualKeyCode.VK_R);
        new KeyboardReader((int)VirtualKeyCode.VK_S);
        new KeyboardReader((int)VirtualKeyCode.VK_T);
        new KeyboardReader((int)VirtualKeyCode.VK_U);
        new KeyboardReader((int)VirtualKeyCode.VK_V);
        new KeyboardReader((int)VirtualKeyCode.VK_W);
        new KeyboardReader((int)VirtualKeyCode.VK_X);
        new KeyboardReader((int)VirtualKeyCode.VK_Y);
        new KeyboardReader((int)VirtualKeyCode.VK_Z);
        
        new KeyboardReader((int)VirtualKeyCode.VK_0);
        new KeyboardReader((int)VirtualKeyCode.VK_1);
        new KeyboardReader((int)VirtualKeyCode.VK_2);
        new KeyboardReader((int)VirtualKeyCode.VK_3);
        new KeyboardReader((int)VirtualKeyCode.VK_4);
        new KeyboardReader((int)VirtualKeyCode.VK_5);
        new KeyboardReader((int)VirtualKeyCode.VK_6);
        new KeyboardReader((int)VirtualKeyCode.VK_7);
        new KeyboardReader((int)VirtualKeyCode.VK_8);
        new KeyboardReader((int)VirtualKeyCode.VK_9);
        
        new KeyboardReader((int)VirtualKeyCode.SPACE);
        new KeyboardReader((int)VirtualKeyCode.MENU);
        new KeyboardReader((int)VirtualKeyCode.ESCAPE);
        new KeyboardReader((int)VirtualKeyCode.BACK);
        new KeyboardReader((int)VirtualKeyCode.RETURN);
        new KeyboardReader((int)VirtualKeyCode.LEFT);
        new KeyboardReader((int)VirtualKeyCode.RIGHT);
        new KeyboardReader((int)VirtualKeyCode.UP);
        new KeyboardReader((int)VirtualKeyCode.DOWN);

        //TOGGLE
        new KeyboardReader((int)VirtualKeyCode.F3);
    }

    public KeyboardReader(int vKey){
        VKey = vKey;
        Keys.Add(vKey, this);
    }
    [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    public static extern short GetKeyState(int nVirtKey);

    public static bool IsKeyDown(int nVirtKey){
        return GetKeyState(nVirtKey) < 0;
    }
    
}
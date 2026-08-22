using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using MultiInput.Enums;
using MultiInput.Enums.Constants;
using MultiInput.TCP;
using WindowsInput;
using WindowsInput.Native;

namespace MultiInput.Inputter;


public class KeyboardInput
{
    private InputSimulator _inputSimulator = new InputSimulator();

    public void Handle(string data,int count){
        if (count > 3) {
            string output = string.Empty;
            for (int i = 0; i < count; i += 3) {
                if (i + 3 < count)
                    output += data.Substring(i, 3) + ".";
                else
                    output += data.Substring(i);
            }
            foreach (var str in output.Split('.')) {
                Console.WriteLine($"orig:{output} for:{str}");
                if (!int.TryParse(str[2].ToString(), out int fState) || !int.TryParse(str.Remove(2), out int fKey)) {
                    Console.WriteLine($"Failed To Handle Data {data} LN 45");
                    return;
                }

                Console.WriteLine($"KEY:{fKey} STATE:{fState}");
                SendKeyStroke(fKey,fState);
            }
            return;
        }

        if (!int.TryParse(data[2].ToString(), out int state) || !int.TryParse(data.Remove(2), out int key)) {
            Console.WriteLine($"Failed To Handle Data {data} LN 45");
            return;
        }

        Console.WriteLine($"KEY:{key} STATE:{state}");
        SendKeyStroke(key,state);
    }
    
    
    
    public void SendKeyStroke(int key,int state){
        if (state == 1) {
            _inputSimulator.Keyboard.KeyDown((VirtualKeyCode)key);
            return;
        }
        _inputSimulator.Keyboard.KeyUp((VirtualKeyCode)key);
    }
}

public class Key
{
    public static Dictionary<int, Key> Keys = new Dictionary<int, Key>();
    public int VKey;
    public bool IsDown;

    public static void UpdateKeys(Socket kTcpHost){
        foreach (var key in Keys.Values) {
            
            bool newState = IsKeyDown(key.VKey);
            if (key.IsDown != newState) {
                
                key.OnKeyStateChange(newState ? KeyStateChanged.JustPressed : KeyStateChanged.JustReleased,kTcpHost);
            }
            key.IsDown = newState;
        }
    }

    public void OnKeyStateChange(KeyStateChanged ev, Socket kTcpHost){
        string data = ev == KeyStateChanged.JustPressed ? $"{VKey}1" : $"{VKey}0";

        var span = Encoding.UTF8.GetBytes(data);

        Console.WriteLine($"sending: |{data}|  length:|{span.Length}| data");
        try {
            kTcpHost.Send(span);
        }
        catch (Exception) {
            kTcpHost.Dispose();
            kTcpHost.Close();
            TcpServer.EndProgram = true;
        }
    }

    public static Key? GetKeyByVKey(int key){
        return Keys.FirstOrDefault(x=>x.Key == key).Value;
    }

    public static void InitializeKeys(){
        new Key((int)VirtualKeyCode.VK_A).Register();
        new Key((int)VirtualKeyCode.VK_B).Register();
        new Key((int)VirtualKeyCode.VK_C).Register();
        new Key((int)VirtualKeyCode.VK_D).Register();
        new Key((int)VirtualKeyCode.VK_E).Register();
        new Key((int)VirtualKeyCode.VK_F).Register();
        new Key((int)VirtualKeyCode.VK_G).Register();
        new Key((int)VirtualKeyCode.VK_H).Register();
        new Key((int)VirtualKeyCode.VK_I).Register();
        new Key((int)VirtualKeyCode.VK_J).Register();
        new Key((int)VirtualKeyCode.VK_K).Register();
        new Key((int)VirtualKeyCode.VK_L).Register();
        new Key((int)VirtualKeyCode.VK_M).Register();
        new Key((int)VirtualKeyCode.VK_N).Register();
        new Key((int)VirtualKeyCode.VK_O).Register();
        new Key((int)VirtualKeyCode.VK_P).Register();
        new Key((int)VirtualKeyCode.VK_Q).Register();
        new Key((int)VirtualKeyCode.VK_R).Register();
        new Key((int)VirtualKeyCode.VK_S).Register();
        new Key((int)VirtualKeyCode.VK_T).Register();
        new Key((int)VirtualKeyCode.VK_U).Register();
        new Key((int)VirtualKeyCode.VK_V).Register();
        new Key((int)VirtualKeyCode.VK_W).Register();
        new Key((int)VirtualKeyCode.VK_X).Register();
        new Key((int)VirtualKeyCode.VK_Y).Register();
        new Key((int)VirtualKeyCode.VK_Z).Register();
        new Key((int)VirtualKeyCode.SPACE).Register();
    }

    public Key(int vKey){
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
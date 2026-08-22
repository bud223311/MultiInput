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

    public void Handle(string data){
        string tData = data.Normalize();
        
        Console.WriteLine($"{data} L:{data.Length} TrimmedCount:{tData.Length} ");
        
        if (!int.TryParse(data[2].ToString(), out int state) || !int.TryParse(data.Remove(2), out int key)) {
            Console.WriteLine($"Failed To Handle Data {data}");
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
    public byte[] OnState;
    public byte[] OffState;
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
        new Key(Enums.Constants.Keys.VK_W,InputType.WOn,InputType.WOff).Register();
        new Key(Enums.Constants.Keys.VK_A,InputType.AOn,InputType.AOff).Register();
        new Key(Enums.Constants.Keys.VK_S,InputType.SOn,InputType.SOff).Register();
        new Key(Enums.Constants.Keys.VK_D,InputType.DOn,InputType.DOff).Register();
    }

    public Key(int vKey,byte[] onState,byte[] offState){
        VKey = vKey;
        OnState = onState;
        OffState = offState;
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
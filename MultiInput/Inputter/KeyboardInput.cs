using System.Runtime.InteropServices;
using MultiInput.Enums;
using MultiInput.Enums.Constants;
using MultiInput.TCP;

namespace MultiInput.Inputter;


public class KeyboardInput
{

    public static void Handle(int data){
        switch (data) {
            case 1:
                WKey();
                break;
            case 2:
                WKey(true);
                break;
            case 3:
                AKey();
                break;
            case 4:
                AKey(true);
                break;
            case 5:
                SKey();
                break;
            case 6:
                SKey(true);
                break;
            case 7:
                DKey();
                break;
            case 8:
                DKey(true);
                break;
        }
    }
    
    
    
    public static void WKey(bool released = false){
        if (!released) {
            Console.WriteLine($"wOn");
            return;
        }
        Console.WriteLine($"wOff");
    }
    public static void AKey(bool release = false){
        
    }
    public static void SKey(bool release = false){
        
    }
    public static void DKey(bool release = false){
    }
    
    
}

public class Key
{
    public static Dictionary<int, Key> Keys = new Dictionary<int, Key>();
    public byte[] OnState;
    public byte[] OffState;
    public int VKey;
    public bool IsDown;

    public static void UpdateKeys(MultiInputTcp.KTcpHost kTcpHost){
        foreach (var key in Keys.Values) {
            bool newState = IsKeyDown(key.VKey);
            if (key.IsDown != newState) {
                key.OnKeyStateChange(newState ? KeyStateChanged.JustPressed : KeyStateChanged.JustReleased,kTcpHost);
            }
        }
    }

    public void OnKeyStateChange(KeyStateChanged ev, MultiInputTcp.KTcpHost kTcpHost){
        switch (ev) {
            case KeyStateChanged.JustPressed:
                kTcpHost.Listener.Server.Send(this.OnState);
                break;
            case KeyStateChanged.JustReleased:
                kTcpHost.Listener.Server.Send(this.OffState);
                break;
        }
    }

    public static Key? GetKeyByVKey(int key){
        return Keys.FirstOrDefault(x=>x.Key == key).Value;
    }

    public static void InitializeKeys(){
        new Key(Enums.Constants.Keys.VK_W,InputType.WOn,InputType.WOff).Register();
        new Key(Enums.Constants.Keys.VK_A,InputType.AOn,InputType.AOff).Register();
        new Key(Enums.Constants.Keys.VK_S,InputType.AOn,InputType.AOff).Register();
        new Key(Enums.Constants.Keys.VK_D,InputType.AOn,InputType.AOff).Register();
    }

    public Key(int vKey,byte[] onState,byte[] offState){
        VKey = vKey;
        OnState = onState;
        OffState = offState;
    }

    public void Register(){
        Keys.Add(this.VKey,this);
    }
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern short GetKeyState(int nVirtKey);

    public static bool IsKeyDown(int nVirtKey){
        return GetKeyState(nVirtKey) != 0;
    }
}
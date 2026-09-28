using System.Net.Sockets;
using System.Runtime.InteropServices;
using MultiInput.Enums.Constants;
using WindowsInput.Native;

namespace MultiInput.Inputter;

public class KeyboardReader : IInputReader
{
    public Dictionary<int, KeyState> Keys = new Dictionary<int, KeyState>();
    public int VKey;
    public bool IsDown;

    public KeyboardReader()
    {
        InitializeKeys();
    }

    public void UpdateBindings(List<Socket> connections){
        foreach (var key in Keys.Values) {
            
            bool newState = IsKeyDown(key.VKey);
            if (key.IsDown != newState) {
                
                key.OnKeyStateChange(newState ? KeyStateChanged.JustPressed : KeyStateChanged.JustReleased, connections);
            }
            key.IsDown = newState;
        }
    }

    public KeyState? GetKeyByVKey(int key){
        return Keys.FirstOrDefault(x=>x.Key == key).Value;
    }
    public void InitializeKeys(){
        VirtualKeyCode[] codes =
            [
            VirtualKeyCode.VK_A,
            VirtualKeyCode.VK_B,
            VirtualKeyCode.VK_C,
            VirtualKeyCode.VK_D,
            VirtualKeyCode.VK_E,
            VirtualKeyCode.VK_F,
            VirtualKeyCode.VK_G,
            VirtualKeyCode.VK_H,
            VirtualKeyCode.VK_I,
            VirtualKeyCode.VK_J,
            VirtualKeyCode.VK_K,
            VirtualKeyCode.VK_L,
            VirtualKeyCode.VK_M,
            VirtualKeyCode.VK_N,
            VirtualKeyCode.VK_O,
            VirtualKeyCode.VK_P,
            VirtualKeyCode.VK_Q,
            VirtualKeyCode.VK_R,
            VirtualKeyCode.VK_S,
            VirtualKeyCode.VK_T,
            VirtualKeyCode.VK_U,
            VirtualKeyCode.VK_V,
            VirtualKeyCode.VK_W,
            VirtualKeyCode.VK_X,
            VirtualKeyCode.VK_Y,
            VirtualKeyCode.VK_Z,

            VirtualKeyCode.VK_0,
            VirtualKeyCode.VK_1,
            VirtualKeyCode.VK_2,
            VirtualKeyCode.VK_3,
            VirtualKeyCode.VK_4,
            VirtualKeyCode.VK_5,
            VirtualKeyCode.VK_6,
            VirtualKeyCode.VK_7,
            VirtualKeyCode.VK_8,
            VirtualKeyCode.VK_9,

            VirtualKeyCode.SPACE,
            VirtualKeyCode.MENU,
            VirtualKeyCode.ESCAPE,
            VirtualKeyCode.BACK,
            VirtualKeyCode.RETURN,
            VirtualKeyCode.LEFT,
            VirtualKeyCode.RIGHT,
            VirtualKeyCode.UP,
            VirtualKeyCode.DOWN,

            VirtualKeyCode.F3
            ];
        foreach (VirtualKeyCode code in codes)
            Keys.Add((int)code, new KeyState((int)code));
    }
    [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    public static extern short GetKeyState(int nVirtKey);

    public static bool IsKeyDown(int nVirtKey){
        return GetKeyState(nVirtKey) < 0;
    }

    public void OnCloseProgram()
    {

    }
}
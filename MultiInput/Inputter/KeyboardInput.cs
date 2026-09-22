using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using MultiInput.Enums;
using MultiInput.Enums.Constants;
using MultiInput.Logging;
using MultiInput.TCP;
using WindowsInput;
using WindowsInput.Native;

namespace MultiInput.Inputter;


public class KeyboardInput
{
    private static InputSimulator _inputSimulator = new InputSimulator();
    private static List<int> PressedKeys = new List<int>();

    public void Handle(string data,int count){
        DebugLog.WriteDebugMessage($"Received: {data}");
        if (count > 3) {
            if (data.Equals("ping", StringComparison.InvariantCultureIgnoreCase)) {
                Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Alive Ping Received");
                DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Alive Ping Received");
                return;
            }
            
            var strs = data.Split('.');


            if (!int.TryParse(strs[0], out int inputType)) {
                Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Input Type. DATA:{data}");
                DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Input Type. DATA:{data}");
                return;
            }
            string fKey = strs[1];
            if (fKey == string.Empty) {
                Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Key. DATA:{data}");
                DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Key. DATA:{data}");
                return;
            }
            if (!int.TryParse(strs[2], out int fState)) {
                Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse State. DATA:{data}");
                DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse State. DATA:{data}");
                return;
            }
            
            if (inputType is (int)InputType.Keyboard) {
                if (!int.TryParse(fKey, out int fKeyInt)) {
                    Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Key. DATA:{data}");
                    DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Key. DATA:{data}");
                    return;
                }
                
                Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| KEY:{fKey} STATE:{fState}");
                DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| KEY:{fKey} STATE:{fState}");
                SendKeyStroke(fKeyInt, fState);
                return;
            }
            
            if (inputType is (int)InputType.Controller) {
                Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| CONTROLLER INPUT | KEY:{fKey} STATE:{fState}");
                DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| CONTROLLER INPUT | KEY:{fKey} STATE:{fState}");
                switch (fKey) {
                    case $"A":
                        SendKeyStroke((int)VirtualKeyCode.SPACE, fState);
                        break;
                    case $"B":
                        SendKeyStroke((int)VirtualKeyCode.LCONTROL, fState);
                        break;
                    case $"X":
                        SendKeyStroke((int)VirtualKeyCode.VK_F, fState);
                        break;
                    case $"Y":
                        SendKeyStroke((int)VirtualKeyCode.VK_V, fState);
                        break;
                    case $"LeftShoulder":
                        SendKeyStroke((int)VirtualKeyCode.VK_Q, fState);
                        break;
                    case $"RightShoulder":
                        SendKeyStroke((int)VirtualKeyCode.VK_E, fState);
                        break;
                    case $"DPadUp":
                        SendKeyStroke((int)VirtualKeyCode.UP, fState);
                        break;
                    case $"DPadDown":
                        SendKeyStroke((int)VirtualKeyCode.DOWN, fState);
                        break;
                    case $"DPadLeft":
                        SendKeyStroke((int)VirtualKeyCode.LEFT, fState);
                        break;
                    case $"DPadRight":
                        SendKeyStroke((int)VirtualKeyCode.RIGHT, fState);
                        break;
                    case $"Start":
                        SendKeyStroke((int)VirtualKeyCode.RETURN, fState);
                        break;
                    case $"Back":
                        SendKeyStroke((int)VirtualKeyCode.ESCAPE, fState);
                        break;
                }
            }
        }
    }

    public static List<int> GetUpVirtualKeys(){
        return PressedKeys;
    }
    
    
    
    public static void SendKeyStroke(int key,int state){
        if (state == 1) {
            PressedKeys.Add(key);
            _inputSimulator.Keyboard.KeyDown((VirtualKeyCode)key);
            return;
        }

        PressedKeys.Remove(key);
        _inputSimulator.Keyboard.KeyUp((VirtualKeyCode)key);
    }

    public static void ReleaseAll(){
        foreach (var virtualKey in PressedKeys) {
            SendKeyStroke(virtualKey,0);
        }
    }
}




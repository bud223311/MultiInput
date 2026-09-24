using MultiInput.Logging;
using WindowsInput.Native;

namespace MultiInput.Inputter;

public static class HandlerUtils
{
    public static Data? ValidateData(string data){
        if (data.Equals("ping", StringComparison.InvariantCultureIgnoreCase)) {
            Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Alive Ping Received");
            DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Alive Ping Received");
            return null;
        }
        
        
        var strs = data.Split('.');
        string fKey = strs[1];
        
        
        if (!int.TryParse(strs[0], out int inputType)) {
            Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Input Type. DATA:{data}");
            DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Input Type. DATA:{data}");
            return null;
        }
            
        if (fKey == string.Empty) {
            Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Key. DATA:{data}");
            DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Key. DATA:{data}");
            return null;
        }
        if (!int.TryParse(strs[2], out int fState)) {
            Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse State. DATA:{data}");
            DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse State. DATA:{data}");
            return null;
        }

        return new Data((InputType)inputType,fKey,fState);
    }

    public static void HandleController(Data data){
        switch (data.Key) {
            /*case $"A":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.SPACE, fState);
                break;
            case $"B":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.LCONTROL, fState);
                break;
            case $"X":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.VK_F, fState);
                break;
            case $"Y":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.VK_V, fState);
                break;
            case $"LeftShoulder":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.VK_Q, fState);
                break;
            case $"RightShoulder":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.VK_E, fState);
                break;
            case $"DPadUp":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.UP, fState);
                break;
            case $"DPadDown":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.DOWN, fState);
                break;
            */case $"DPadLeft":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.VK_F, data.State);
                break;/*
            case $"DPadRight":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.RIGHT, fState);
                break;
            case $"Start":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.RETURN, fState);
                break;
            case $"Back":
                KeyboardInput.SendKeyStroke((int)VirtualKeyCode.ESCAPE, fState);
                break;*/
            default:
                break;
        }
    }
}
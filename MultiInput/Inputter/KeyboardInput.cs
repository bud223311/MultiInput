using MultiInput.Configuration;
using MultiInput.Logging;
using WindowsInput;
using WindowsInput.Native;

namespace MultiInput.Inputter;


public class KeyboardInput
{
    private static InputSimulator _inputSimulator = new InputSimulator();
    private static List<int> _pressedKeys = new List<int>();

    public void Handle(string data,int count,ConfigurationFile config){
        DebugLog.WriteDebugMessage($"Received: {data}");
        if (count > 3) {
            
            if (!HandlerUtils.TryParseData(data, out Data? validData)){
                return;
            }
            Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| {validData.InputType} INPUT | KEY:{validData.Key} STATE:{validData.State}");
            DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| {validData.InputType} INPUT | KEY:{validData.Key} STATE:{validData.State}");

            switch (validData.InputType) {
                case InputType.Keyboard:
                {
                    if (!int.TryParse(validData.Key, out int fKeyInt)) {
                        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Key. DATA:{data}");
                        DebugLog.WriteDebugMessage($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}| Failed To Parse Key. DATA:{data}");
                        return;
                    }
                    ConfigurationEntry? configurationEntry = config.ClientConfigs.FirstOrDefault(x => x.ReplacingKey != null &&
                        x.ReplacingKey == fKeyInt);
        
                    if (configurationEntry is null) {
                        DebugLog.DebugMessageThread($"No configuration found for Keyboard button: {fKeyInt}");
                        return;
                    }
                    
                    SendKeyStroke(configurationEntry.ReplacedKey, validData.State);
                    return;
                }
                case InputType.Controller:
                    HandlerUtils.HandleController(validData, config);
                    break;
            }
        }
    }

    public static List<int> GetUpVirtualKeys(){
        return _pressedKeys;
    }
    
    public static void SendKeyStroke(int key,int state){
        if (state == 1) {
            _pressedKeys.Add(key);
            _inputSimulator.Keyboard.KeyDown((VirtualKeyCode)key);
            return;
        }

        _pressedKeys.Remove(key);
        _inputSimulator.Keyboard.KeyUp((VirtualKeyCode)key);
    }

    public static void ReleaseAll(){
        foreach (var virtualKey in _pressedKeys) {
            SendKeyStroke(virtualKey,0);
        }
    }
}




using MultiInput.Configuration;
using MultiInput.Logging;
using SDL3;
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

    public static bool TryParseData(string data, out Data? parsedData)
    {
        parsedData = ValidateData(data);
        return parsedData is not null;
    }

    public static void HandleController(Data data,ConfigurationFile config){

        if (SDL.GamepadButton.TryParse(data.Key, out SDL.GamepadButton button)) {
            DebugLog.DebugMessageThread($"Failed to parse controller button: {data.Key}");
            return;
        }
        ConfigurationEntry? configurationEntry = config.ClientConfigs.FirstOrDefault(x => x.ReplacingControllerKey != null &&
                                                                                    x.ReplacingControllerKey == button);
        
        if (configurationEntry is null) {
            DebugLog.DebugMessageThread($"No configuration found for controller button: {data.Key}");
            return;
        }
        KeyboardInput.SendKeyStroke(configurationEntry.ReplacedKey, data.State);
    }
}
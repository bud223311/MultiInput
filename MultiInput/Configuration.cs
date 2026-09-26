using MultiInput.Inputter;
using MultiInput.Logging;

namespace MultiInput;

public class Configuration
{
    public List<ConfigEntry> AllConfigEntries = new List<ConfigEntry>();
    public static readonly string ConfigLocation = $@"{Environment.CurrentDirectory}\Config";
    public static readonly string ConfigFile = $@"{ConfigLocation}\MultiInputConfig.txt";
    //TODO Test if Ts even works cause I'm too trtirted Zzz

    public Configuration()
    {
        ReadConfig();
    }
    
    public bool ReadConfig(){
        ConsoleLog.WriteConsoleMessage($"Reading ConfigFile");
        if (!VerifyExists()) {
            WriteFreshConfig();
        }
        StreamReader sr = new StreamReader(ConfigFile);
        try {
            while (!sr.EndOfStream) {
                string? line = sr.ReadLine();
                if (line is null) {
                    continue;
                }

                if (line.StartsWith(COMMENT_OPERATOR)) {
                    continue;
                }

                string[] arguments = line.Split('.');
                ConfigEntry entry = new ConfigEntry(arguments);
                AllConfigEntries.Add(entry);
            }
        }
        catch (OutOfMemoryException e) {
            ConsoleLog.WriteConsoleMessage($"OutOfMemoryException (ram prices my beloved)\n{e}");
        }

        return true;
    }
    const string COMMENT_OPERATOR = "//";
    const string CONFIG_DEFAULT_CONTENT = $"{COMMENT_OPERATOR} Here you can write new ConfigEntries that will modify data once it arrives\n" +
                                        $"{COMMENT_OPERATOR} For example below here would replace sending keyboard data from 66 to 74 which is 'B' to 'J'\n" +
                                        $"{COMMENT_OPERATOR} Sender.Keyboard.66.74\n" +
                                        $"{COMMENT_OPERATOR} Which means when you press 'B' it will send info to the other clients that you pressed 'J'\n" +
                                        $"{COMMENT_OPERATOR} Here is the main use of the program when you receive data. For example: DPadLeft from a controller or a number from a keyboard\n" +
                                        $"{COMMENT_OPERATOR} You can make the data you receive send a keystroke\n" +
                                        $"{COMMENT_OPERATOR} Receiver.Controller.DPadLeft.81\n" +
                                        $"{COMMENT_OPERATOR} This will press button VirtualKey 81 on a keyboard when receiving DPadLeft\n" +
                                        $"{COMMENT_OPERATOR} You can write your configurations below this one without '{COMMENT_OPERATOR}'";

    public void WriteFreshConfig(){
        ConsoleLog.WriteConsoleMessage($"Rewriting Default Config");
        ClearFile();
        StreamWriter sw = new StreamWriter(ConfigFile);
        sw.WriteLine(CONFIG_DEFAULT_CONTENT);
        sw.Close();
    }

    private void ClearFile(){
        VerifyConfig();
        File.Delete(ConfigFile);
        File.Create(ConfigFile).Close();
    }

    public void VerifyConfig(){
        Directory.CreateDirectory(ConfigLocation);

        if (!File.Exists(ConfigFile)) {
            File.Create(ConfigFile).Close();
        }
    }

    public bool VerifyExists(){
        return Directory.Exists(ConfigLocation) && File.Exists(ConfigFile);
    }
}

public class ConfigEntry
{
    public DataConfigHandlingType DataConfigHandlingType;
    public InputType InputType;
    public int? ReplacingKey;
    public string? ReplacingControllerKey;
    public int ReplacedKey;
        
        
    public int Key;
    public ConfigEntry(string[] configLine){
        ConsoleLog.WriteConsoleMessage($"attempted Adding Config Entry\nVALUE: {string.Join('.',configLine)}",ConsoleColor.Yellow);
        if (configLine is{ Length: < 4 }) {
            DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[0]}| as valid DataConfigHandlingType");
        }
        
        switch (configLine[0].ToLowerInvariant()) {
            case $"sender":
                DataConfigHandlingType = DataConfigHandlingType.Sender;
                break;
            case $"receiver":
                DataConfigHandlingType = DataConfigHandlingType.Receiver;
                break;
            default:
                DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[0]}| as valid DataConfigHandlingType");
                return;
        }

        switch (configLine[1].ToLowerInvariant()) {
            case $"controller" or $"c":
                InputType = InputType.Controller;
                if (!Enum.GetNames<ControllerReader.XInputButton>().Any(x => x.Equals(configLine[2], StringComparison.InvariantCultureIgnoreCase))) {
                    DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[2]}| as valid InputType");
                    return;
                }
                ReplacingControllerKey = configLine[2];
                
                
                break;
            case $"keyboard" or $"k":
                InputType = InputType.Keyboard;
                
                if (!int.TryParse(configLine[2],out int replacingKey)) {
                    DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[2]}| as valid InputType");
                    return;
                }
                ReplacingKey = replacingKey;
                break;
            default:
                DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[1]}| as valid InputType");
                return;
        }
        
        if (!int.TryParse(configLine[3],out int replacedKey)) {
            DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[2]}| as valid InputType");
            return;
        }

        ReplacedKey = replacedKey;
        ConsoleLog.WriteConsoleMessage($"Accepted",ConsoleColor.Green);
    }
}

public enum DataConfigHandlingType
{
    Sender,
    Receiver
}
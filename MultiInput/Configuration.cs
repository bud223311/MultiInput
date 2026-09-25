using MultiInput.Inputter;
using MultiInput.Logging;
using MultiInput.Timer;

namespace MultiInput;

public static class Configuration
{
    public static readonly string ConfigLocation = $@"{Environment.CurrentDirectory}\Config";
    public static readonly string ConfigFile = $@"{ConfigLocation}\MultiInputConfig.txt";
    //TODO Test if Ts even works cause I'm too trtirted Zzz
    
    public static bool ReadConfig(){
        ConsoleLog.WriteConsoleMessage($"Reading ConfigFile");
        if (!VerifyExists()) {
            WriteFreshConfig();
        }
        StreamReader sr = new StreamReader(ConfigFile);
        try {
            string? reinitializationLine = sr.ReadLine();
            if (reinitializationLine is null || !reinitializationLine.StartsWith('*')) {
                sr.Dispose();
                sr.Close();
                WriteFreshConfig();
                return false;
            }

            while (!sr.EndOfStream) {
                string? line = sr.ReadLine();
                if (line is null) {
                    continue;
                }

                if (line.StartsWith($"//")) {
                    continue;
                }

                new ConfigEntry(line.Split('.'));


            }
        }
        catch (OutOfMemoryException e) {
            ConsoleLog.WriteConsoleMessage($"OutOfMemoryException (ram prices my beloved)\n{e}");
        }

        return true;
    }

    public static void WriteFreshConfig(){
        ConsoleLog.WriteConsoleMessage($"Rewriting Default Config");
        ClearFile();
        var sw = new StreamWriter(ConfigFile);
        sw.WriteLine($"* WARNING: Removing This Line Will Regenerate this Config Which will delete all configs that are in here");
        sw.WriteLine($"//Here you can Write new ConfigEntries that will modify Data Once it arrives");
        sw.WriteLine($"//For Example Below here would replace Sending Keyboard Data from 66 to 74 which is 'B' to 'J'");
        sw.WriteLine($"//Sender.Keyboard.66.74");
        sw.WriteLine($"//Which Means when you press 'B' it will send info to the other clients that you pressed 'J'");
        sw.WriteLine($"//Here is the main use of the program when you Receive Data. For Example: DPadLeft from a controller or a number from a keyboard");
        sw.WriteLine($"//You can make the data you receive send a keystroke");
        sw.WriteLine($"//Receiver.Controller.DPadLeft.81");
        sw.WriteLine($"//This will press button VirtualKey 81 on a keyboard when Receiving DPadLeft");
        sw.WriteLine($"\n\n//Write new lines above this one");
        sw.Close();
    }

    private static void ClearFile(){
        VerifyConfig();
        File.Delete(ConfigFile);
        File.Create(ConfigFile).Close();
    }

    public static void VerifyConfig(){
        if (!Directory.Exists(ConfigLocation)) {
            Directory.CreateDirectory(ConfigLocation);
        }

        if (!File.Exists(ConfigFile)) {
            File.Create(ConfigFile).Close();
        }
    }

    public static bool VerifyExists(){
        return Directory.Exists(ConfigLocation) && File.Exists(ConfigFile);
    }
}

public class ConfigEntry
{
    public static List<ConfigEntry> AllConfigEntries = new List<ConfigEntry>();
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
        AllConfigEntries.Add(this);
    }
}

public enum DataConfigHandlingType
{
    Sender,
    Receiver
}
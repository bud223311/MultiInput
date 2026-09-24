using MultiInput.Logging;

namespace MultiInput;

public static class Configuration
{
    public static readonly string ConfigLocation = $@"{Environment.CurrentDirectory}\Config";
    public static readonly string ConfigFile = $@"{ConfigLocation}\MultiInputConfig.txt";
    public static StreamReader? StreamReader;
    //TODO Test if Ts even works cause I'm too trtirted Zzz
    
    public static void ReadConfig(){
        StreamReader = new StreamReader(ConfigFile);
        try {
            string? reinitializationLine = StreamReader.ReadLine();
            if (reinitializationLine is null || !reinitializationLine.StartsWith('*')) {
                WriteFreshConfig();
                return;
            }

            while (!StreamReader.EndOfStream) {
                string? line = StreamReader.ReadLine();
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
    }

    public static void WriteFreshConfig(){
        if (StreamReader is not null) {
            StreamReader.Close();
            StreamReader.Dispose();
            StreamReader = null;
        }
        
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
        
        
    }

    private static void ClearFile(){
        VerifyConfig();
        File.Delete(ConfigFile);
        File.Create(ConfigFile);
    }

    public static void VerifyConfig(){
        if (!Directory.Exists(ConfigLocation)) {
            Directory.CreateDirectory(ConfigLocation);
        }

        if (!File.Exists(ConfigFile)) {
            File.Create(ConfigFile);
        }
    }
}

public class ConfigEntry
{
    public static List<ConfigEntry> AllConfigLines = new List<ConfigEntry>();
    public DataConfigHandlingType DataConfigHandlingType;
    public InputType InputType;
    public int Key;
    public ConfigEntry(string[] configLine){
        
        switch (configLine[0].ToLowerInvariant()) {
            case $"sender":
                DataConfigHandlingType = DataConfigHandlingType.Sender;
                break;
            case $"receiver":
                DataConfigHandlingType = DataConfigHandlingType.Receiver;
                break;
            default:
                DebugLog.DebugMessageThread($"Failed To Parse |{configLine[0]}| as valid DataConfigHandlingType");
                break;
        }

        switch (configLine[1].ToLowerInvariant()) {
            case $"controller" or $"c":
                InputType = InputType.Controller;
                break;
            case $"keyboard" or $"k":
                InputType = InputType.Keyboard;
                break;
            default:
                DebugLog.DebugMessageThread($"Failed To Parse |{configLine[1]}| as valid InputType");
                break;
        }
    }
}

public enum DataConfigHandlingType
{
    Sender,
    Receiver
}
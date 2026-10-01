using MultiInput.Logging;

namespace MultiInput.Configuration;

public class ConfigurationFile
{
    public List<ConfigurationEntry> ClientConfigs = new List<ConfigurationEntry>();
    public List<ConfigurationEntry> HostConfigs = new List<ConfigurationEntry>();

    public static readonly string ConfigLocation = $@"{Environment.CurrentDirectory}\Config";
    public static readonly string ConfigFile = $@"{ConfigLocation}\MultiInputConfig.txt";
    //TODO Test if Ts even works cause I'm too trtirted Zzz

    public ConfigurationFile()
    {
        if (!VerifyExists())
        {
            WriteFreshConfig();
        }

        ReadConfig();
    }

    public void ReadConfigLine(StreamReader file)
    {
        string? line = file.ReadLine();
        if (line is null)
        {
            return;
        }

        if (line.StartsWith(COMMENT_OPERATOR))
        {
            return;
        }

        string[] arguments = line.Split('.');
        ConfigurationEntry entry = new ConfigurationEntry(arguments);
        switch (entry.DataConfigHandlingType)
        {
            case DataConfigurationHandlingType.Receiver:
                ClientConfigs.Add(entry);
                break;
            case DataConfigurationHandlingType.Sender:
                HostConfigs.Add(entry);
                break;
        }
    }
    
    public bool ReadConfig(){
        ConsoleLog.WriteConsoleMessage($"Reading configuration file.");
        StreamReader sr = new StreamReader(ConfigFile);
        while (!sr.EndOfStream)
        {
            ReadConfigLine(sr);
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



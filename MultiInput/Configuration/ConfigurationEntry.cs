using MultiInput.Inputter;
using MultiInput.Logging;

namespace MultiInput.Configuration
{
    public class ConfigurationEntry
    {
        public DataConfigHandlingType DataConfigHandlingType;
        public InputType InputType;
        public int? ReplacingKey;
        public string? ReplacingControllerKey;
        public int ReplacedKey;


        public int Key;
        public ConfigurationEntry(string[] configLine)
        {
            ConsoleLog.WriteConsoleMessage($"Attempting adding configuration entry\nVALUE: {string.Join('.', configLine)}", ConsoleColor.Yellow);
            if (configLine is { Length: < 4 })
            {
                DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[0]}| as valid DataConfigHandlingType");
            }

            switch (configLine[0].ToLowerInvariant())
            {
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

            switch (configLine[1].ToLowerInvariant())
            {
                case $"controller" or $"c":
                    InputType = InputType.Controller;
                    if (!Enum.GetNames<ControllerReader.XInputButton>().Any(x => x.Equals(configLine[2], StringComparison.InvariantCultureIgnoreCase)))
                    {
                        DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[2]}| as valid InputType");
                        return;
                    }
                    ReplacingControllerKey = configLine[2];


                    break;
                case $"keyboard" or $"k":
                    InputType = InputType.Keyboard;

                    if (!int.TryParse(configLine[2], out int replacingKey))
                    {
                        DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[2]}| as valid InputType");
                        return;
                    }
                    ReplacingKey = replacingKey;
                    break;
                default:
                    DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[1]}| as valid InputType");
                    return;
            }

            if (!int.TryParse(configLine[3], out int replacedKey))
            {
                DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[2]}| as valid InputType");
                return;
            }



            ReplacedKey = replacedKey;
            ConsoleLog.WriteConsoleMessage($"Accepted configuration entry", ConsoleColor.Green);
        }
    }
}

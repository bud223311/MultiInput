
using MultiInput.Logging;
using SDL3;

namespace MultiInput.Configuration
{
    public class ConfigurationEntry
    {
        public DataConfigurationHandlingType DataConfigHandlingType;
        public InputType InputType;
        public int? ReplacingKey;
        public SDL.GamepadButton? ReplacingControllerKey;
        public int ReplacedKey;
        public int Key;
        const int HANDLING_TYPE_INDEX = 0;
        const int INPUT_TYPE_INDEX = 1;
        const int REPLACING_BINDING_INDEX = 2;
        const int REPLACED_BINDING_INDEX = 3;

        public bool TryGetHandlingType(string input, out DataConfigurationHandlingType type)
        {
            return DataConfigurationHandlingType.TryParse(input, ignoreCase: true, out type);
        }

        public bool TryGetInputType(string input, out InputType type)
        {
            switch(input)
            {
                case "k":
                    {
                        input = "Keyboard";
                        break;
                    }
                case "c":
                    {
                        input = "Controller";
                        break;
                    }
            }

            return InputType.TryParse(input, ignoreCase: true, out type);
        }
        public bool TryGetReplacingBinding(string input, out object binding)
        {
            binding = default;
            switch(InputType)
            {
                case InputType.Keyboard:
                    {
                        bool result = int.TryParse(input, out int bindingKey);
                        binding = bindingKey;
                        return result;
                    }
                case InputType.Controller:
                    {
                        bool result = Enum.TryParse(input, ignoreCase: true, out SDL.GamepadButton controllerBinding);
                        binding = controllerBinding;
                        return result;
                    }
                default:
                    {
                        DebugLog.WriteDebugMessage($"Unknown input type was defined in the configuration entry, aborting..");
                        return false;
                    }
            }
        }

        public bool TryGetReplacedBinding(string input, out int replacedBinding)
        {
            return int.TryParse(input, out replacedBinding);
        }
        public ConfigurationEntry(string[] configLine)
        {
            ConsoleLog.WriteConsoleMessage($"Attempting adding configuration entry\nVALUE: {string.Join('.', configLine)}", ConsoleColor.Yellow);
            if (configLine is { Length: < 4 })
            {
                DebugLog.WriteDebugMessage($"Not enough parameters in the configuration entry to be valid, ignoring the line..");
                return;
            }

            for(int i = 0; i < configLine.Length; i++)
            {
                switch(i)
                {
                    case HANDLING_TYPE_INDEX:
                        {
                            if (!TryGetHandlingType(configLine[i], out DataConfigurationHandlingType type))
                            {
                                DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[i]}| as valid DataConfigHandlingType");
                                return;
                            }
                            DataConfigHandlingType = type;
                            break;
                        }
                    case INPUT_TYPE_INDEX:
                        {
                            if (!TryGetInputType(configLine[i], out InputType type))
                            {
                                DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[i]}| as valid InputType");
                                return;
                            }
                            InputType = type;
                            break;
                        }
                    case REPLACING_BINDING_INDEX:
                        {
                            if (!TryGetReplacingBinding(configLine[i], out object binding))
                            {
                                DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[i]}| as valid replacing input binding");
                                return;
                            }
                            switch(InputType)
                            {
                                case InputType.Keyboard:
                                    {
                                        ReplacingKey = (int)binding;
                                        break;
                                    }
                                case InputType.Controller:
                                    {
                                        ReplacingControllerKey = (SDL.GamepadButton)binding;
                                        break;
                                    }
                            }
                            break;
                        }
                    case REPLACED_BINDING_INDEX:
                        {
                            if (!TryGetReplacedBinding(configLine[i], out int binding))
                            {
                                DebugLog.WriteDebugMessage($"Failed To Parse |{configLine[i]}| as valid replaced input binding");
                                return;
                            }
                            ReplacedKey = binding;
                            break;
                        }
                    default:
                        {
                            DebugLog.WriteDebugMessage($"Unknown extra parameter added to the configuration entry, ignoring...");
                            break;
                        }
                }

            }
            ConsoleLog.WriteConsoleMessage($"Accepted configuration entry", ConsoleColor.Green);
        }
    }
}

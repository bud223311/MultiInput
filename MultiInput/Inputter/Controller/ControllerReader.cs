
using MultiInput.ControllerUtils;
using MultiInput.Logging;
using MultiInput.Timer;
using System.Net.Sockets;
using MultiInput.Configuration;
using MultiInput.Enums.Constants;
using SDL3;

namespace MultiInput.Inputter.Controller;

public class ControllerReader : IInputReader
{
    public Dictionary<int, ControllerKeyState> Keys = new Dictionary<int, ControllerKeyState>();

    public IntPtr TargettedControllerIndex;
    private bool _restrictLogSpamming;
    private bool _hasvibratedController;
    

    public ControllerReader()
    {
        InitializeButtons();
        _hasvibratedController = false;
        _restrictLogSpamming = false;
        TargettedControllerIndex = IntPtr.Zero;
        TryGetController();
    }

    public void ReadSdlController(List<Socket> connections, ConfigurationFile config){
        SDL.UpdateGamepads();
        foreach (var key in Keys) {
            bool newState =SDL.GetGamepadButton(TargettedControllerIndex, key.Value.GamepadButton);
            // Update the key state based on the SDL button state
            if (key.Value.IsDown != newState) {
                key.Value.OnKeyStateChange(newState ? KeyStateChanged.JustPressed : KeyStateChanged.JustReleased, connections,config);
            }
            key.Value.IsDown = newState;
        }
    }

    

    public void InitializeButtons(){
        SDL.GamepadButton[] buttons = Enum.GetValues<SDL.GamepadButton>();
        foreach (SDL.GamepadButton button in buttons)
            Keys.Add((int)button, new ControllerKeyState(button));
    }
    
    
    

    public IntPtr? TryGetController(){
        uint? gamepadId = SDL.GetGamepads(out int count)?.FirstOrDefault();
        if (gamepadId is null) {
            return null;
        }
        
        IntPtr sdlGamePadId = SDL.GetGamepadFromID((uint)gamepadId);
        if (sdlGamePadId == IntPtr.Zero) {
            sdlGamePadId = SDL.OpenGamepad((uint)gamepadId);
        }

        return sdlGamePadId;
    }


    public void UpdateBindings(List<Socket> connections,ConfigurationFile config){
        IntPtr? sdlGamePadId = TryGetController();
        if (sdlGamePadId is null) {
            TargettedControllerIndex = IntPtr.Zero;
            if (!_restrictLogSpamming) {
                DebugLog.DebugMessageThread($"No Controller Found");
                ConsoleLog.WriteConsoleMessage($"No Controller Found");
                _restrictLogSpamming = true;
            }
            _hasvibratedController = false;
            return;
        }

        if (!_hasvibratedController) {
            ControllerUtilities.CreateThreadedControllerVibrationStartup((IntPtr)sdlGamePadId);
            _hasvibratedController = true;
        }
        TargettedControllerIndex = (IntPtr)sdlGamePadId;
        ReadSdlController(connections,config);
        
    }

    public void OnCloseProgram()
    {
        SDL.CloseGamepad(TargettedControllerIndex);
    }
}
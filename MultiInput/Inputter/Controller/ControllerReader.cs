using DualSenseAPI;
using DualSenseAPI.State;
using MultiInput.ControllerUtils;
using MultiInput.Inputter.Controller.XInputController;
using MultiInput.Logging;
using MultiInput.Timer;
using System.Net.Sockets;
namespace MultiInput.Inputter.Controller;

public class ControllerReader : IInputReader
{
    public List<ButtonState> AllButtons = new List<ButtonState>();
    public int TargettedControllerIndex = -1;
    internal ControllerInputType ControllerInputType;
    private bool _restrictLogSpamming;
    private bool _hasvibratedController;
    internal DualSense? DualSense
    {
        get;
        set
        {
            if (value is null)
            {
                ControllerInputType = ControllerInputType.XInput;
                InitializeButtons();
                field = null;
                return;
            }
            field = value;
            field.Acquire();
            field.OnButtonStateChanged += OnDualShockControllerStateChange;
            field.BeginPolling(20);
            field.OutputState.LeftRumble = 1f;
            field.OutputState.RightRumble = 1f;
            WaitingTimer.WaitForMilliseconds(500);
            field.OutputState.LeftRumble = 0f;
            field.OutputState.RightRumble = 0f;
            ControllerInputType = ControllerInputType.DualSense;
        }
    }

    public ControllerReader()
    {
        _hasvibratedController = false;
        _restrictLogSpamming = false;
        TargettedControllerIndex = -1;
        TryGetController();
    }

    public void TryGetController(){
        DualSense? dualSense = DualSense.EnumerateControllers().FirstOrDefault();
        DualSense = dualSense;
    }

    private static void OnDualShockControllerStateChange(DualSense sender, DualSenseInputStateButtonDelta changes)
    {
        if (!changes.HasChanges) return;
        OnDualShockControllerButtonState(changes.DPadLeftButton, XInputButton.DPadLeft);
    }

    private static void OnDualShockControllerButtonState(ButtonDeltaState delta, XInputButton button)
    {
        switch (delta)
        {
            case ButtonDeltaState.Pressed:
                ButtonState.SendButtonPacket(InputType.Controller, button, held: true);
                break;
            case ButtonDeltaState.Released:
                ButtonState.SendButtonPacket(InputType.Controller, button, held: false);
                break;
        }
    }

    public void InitializeButtons()
    {
        for (int i = 0; i < XInput.BUTTON_COUNT; i++)
        {
            int buttonindex = i;
            XInputButton button = (XInputButton)(1 << buttonindex);
            ButtonState buttonState = new ButtonState(button);
            AllButtons.Add(buttonState);
        }
    }

    public void ReadAllButtons(int targettedControllerIndex)
    {
        foreach (var buttonState in AllButtons)
        {
            buttonState.IsPressed = XInput.GetButton((uint)targettedControllerIndex, buttonState.Button);

            if (buttonState.IsPressed == !buttonState.WasPressedLastFrame)
            {
                buttonState.NotifyButtonStateChange();
            }

            buttonState.WasPressedLastFrame = buttonState.IsPressed;
        }
    }
    public void UpdateBindings(List<Socket> connections)
    {
        if (ControllerInputType == ControllerInputType.DualSense)
        {
            return;
        }
        int id = XInput.GetFirstConnectedController();
        TargettedControllerIndex = id;
        if (id is -1)
        {
            if (_restrictLogSpamming)
            {
                return;
            }

            _hasvibratedController = false;
            _restrictLogSpamming = true;
            ConsoleLog.WriteConsoleMessage($"No Controller Connected");
            DebugLog.DebugMessageThread($"No Controller Connected");
            return;
        }

        if (!_hasvibratedController)
        {
            _hasvibratedController = true;
            ControllerUtilities.CreateThreadedControllerVibrationStartup(ControllerInputType.XInput, DualSense, TargettedControllerIndex);
        }

        _restrictLogSpamming = false;
        ReadAllButtons(TargettedControllerIndex);
    }

    public void OnCloseProgram()
    {
        DualSense?.EndPolling();
        DualSense?.Release();
    }
}
using DualSenseAPI;
using MultiInput.Inputter.Controller.XInputController;
using MultiInput.Timer;

namespace MultiInput.ControllerUtils;

public static class ControllerUtilities
{
    public static void CreateThreadedControllerVibrationStartup(ControllerInputType controllerInputType, DualSense? dualSense, int targettedControllerIndex){
        new Thread(() => ThreadedControllerVibrationStartup(controllerInputType, dualSense, targettedControllerIndex)).Start();
    }

    private static void ThreadedControllerVibrationStartup(ControllerInputType controllerInputType, DualSense? dualSense, int targettedControllerIndex)
    {
        switch (controllerInputType) {
            case ControllerInputType.DualSense:
            {
                if (dualSense is null) {
                    return;
                }
                dualSense.OutputState.LeftRumble = 1f;
                dualSense.OutputState.RightRumble = 1f;
                WaitingTimer.WaitForMilliseconds(500);
                dualSense.OutputState.LeftRumble = 0f;
                dualSense.OutputState.RightRumble = 0f;
                break;
            }
            case ControllerInputType.XInput:
            {
                XInput.SetVibration((uint)targettedControllerIndex,1f,1f);
                WaitingTimer.WaitForMilliseconds(500);
                XInput.SetVibration((uint)targettedControllerIndex,0f,0f);
                break;
            }
        }
    }
}
using MultiInput.Inputter;
using MultiInput.Timer;

namespace MultiInput.ControllerUtils;

public static class ControllerUtilities
{
    public static void CreateThreadedControllerVibrationStartup(ControllerInputType controllerInputType){
        new Thread(() => ThreadedControllerVibrationStartup(controllerInputType));
    }

    private static void ThreadedControllerVibrationStartup(ControllerInputType controllerInputType){
        switch (controllerInputType) {
            case ControllerInputType.DualSense:
            {
                if (StaticData.DualSense is null) {
                    return;
                }
                StaticData.DualSense.OutputState.LeftRumble = 1f;
                StaticData.DualSense.OutputState.RightRumble = 1f;
                WaitingTimer.WaitForMilliseconds(500);
                StaticData.DualSense.OutputState.LeftRumble = 0f;
                StaticData.DualSense.OutputState.RightRumble = 0f;
                break;
            }
            case ControllerInputType.XInput:
            {
                ControllerReader.XInput.SetVibration((uint)ControllerReader.TargettedControllerIndex,1f,1f);
                WaitingTimer.WaitForMilliseconds(500);
                ControllerReader.XInput.SetVibration((uint)ControllerReader.TargettedControllerIndex,0f,0f);
                break;
            }
        }
    }
}
using MultiInput.Timer;
using SDL3;

namespace MultiInput.ControllerUtils;

public static class ControllerUtilities
{
    public static void CreateThreadedControllerVibrationStartup(IntPtr targettedControllerIndex){
        new Thread(() => ThreadedControllerVibrationStartup(targettedControllerIndex)).Start();
    }

    private static void ThreadedControllerVibrationStartup(IntPtr targettedControllerIndex)
    {
        SDL.RumbleGamepad(targettedControllerIndex, 65535, 65535,500);
        WaitingTimer.WaitForMilliseconds(500);
        SDL.RumbleGamepad(targettedControllerIndex, 0, 0,0);
    }
}
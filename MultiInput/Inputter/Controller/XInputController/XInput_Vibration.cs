using System.Runtime.InteropServices;

namespace MultiInput.Inputter.Controller.XInputController
{
    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_VIBRATION
    {
        public ushort LeftMotorSpeed;
        public ushort RightMotorSpeed;
    }
}

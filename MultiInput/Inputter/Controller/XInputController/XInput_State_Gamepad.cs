using System.Runtime.InteropServices;

namespace MultiInput.Inputter.Controller.XInputController
{

    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_STATE_GAMEPAD
    {
        public uint PacketNumber;
        public ushort Buttons;
        public byte LeftTrigger;
        public byte RightTrigger;
        public short ThumbLX;
        public short ThumbLY;
        public short ThumbRX;
        public short ThumbRY;
    }
}

using System.Numerics;

namespace MultiInput.Inputter.Controller.XInputController
{
    public class XInputState
    {
        public void CopyTo(XInputState target)
        {
            target.IsConnected = IsConnected;
            target.PacketNumber = PacketNumber;
            target.Buttons = Buttons;
            target.TriggerLeft = TriggerLeft;
            target.TriggerRight = TriggerRight;
            target.ThumbLeft = ThumbLeft;
            target.ThumbRight = ThumbRight;
            target.ThumbLeftRaw = ThumbLeftRaw;
            target.ThumbRightRaw = ThumbRightRaw;
        }

        public void Reset()
        {
            IsConnected = false;
            PacketNumber = 0;
            Buttons = 0;
            TriggerLeft = 0.0f;
            TriggerRight = 0.0f;
            ThumbLeft = new Vector2();
            ThumbRight = new Vector2();
            ThumbLeftRaw = new Vector2();
            ThumbRightRaw = new Vector2();
        }


        public bool IsConnected;
        public uint PacketNumber;
        public ushort Buttons;
        public float TriggerLeft;
        public float TriggerRight;
        public Vector2 ThumbLeft;
        public Vector2 ThumbRight;
        public Vector2 ThumbLeftRaw;
        public Vector2 ThumbRightRaw;
    }
}

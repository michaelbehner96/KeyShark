using System.Runtime.InteropServices;

namespace KeyShark.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public class KeyboardData
    {
        public VKey vkCode;
        public uint scanCode;
        public KeyboardDataFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }
}
using System.Runtime.InteropServices;

namespace KeyShark
{
    public static class Pinvoke
    {
        public const int WH_KEYBOARD_LL = 13;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, KeyboardMessage keyboardMessage, IntPtr keyboardDataPtr);
    }

    public enum KeyboardMessage : int
    {
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        SystemKeyDown = 0x0104,
        SystemKeyUp = 0x0105
    }

    [StructLayout(LayoutKind.Sequential)]
    public class KeyboardData
    {
        public VKey vkCode;
        public uint scanCode;
        public KeyboardDataFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [Flags]
    public enum KeyboardDataFlags : uint
    {
        Extended = 0x01,
        Injected = 0x10,
        AltDown = 0x20,
        Up = 0x80,
    }
}
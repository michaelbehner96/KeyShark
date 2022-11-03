using KeyShark.Native;
using Microsoft.Win32.SafeHandles;

namespace KeyShark
{
    public class HookSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private HookSafeHandle() : base(true) { }

        protected override bool ReleaseHandle()
        {
            return Pinvoke.UnhookWindowsHookEx(handle);
        }
    }
}
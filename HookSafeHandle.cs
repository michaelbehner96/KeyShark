using KeyShark.Native;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

namespace KeyShark
{
    public class HookSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private HookSafeHandle() : base(true) { }

#pragma warning disable SYSLIB0004 // Type or member is obsolete
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#pragma warning restore SYSLIB0004 // Type or member is obsolete
        protected override bool ReleaseHandle()
        {
            return Pinvoke.UnhookWindowsHookEx(handle);
        }
    }
}
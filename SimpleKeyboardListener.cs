using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using KeyShark.Native;

namespace KeyShark
{
    public class SimpleKeyboardListener : IKeyboardListener, IDisposable
    {
        public event EventHandler<KeyboardEventArgs>? KeyUp;
        public event EventHandler<KeyboardEventArgs>? KeyDown;
        public event EventHandler<KeyboardEventArgs>? KeyHeld;

        private HookSafeHandle nativeHookHandle;
        private readonly Pinvoke.LowLevelKeyboardProc lowLevelEventDelegate;
        private readonly IKeyStateTracker keyStateTracker;

        public SimpleKeyboardListener(IKeyStateTracker keyStateTracker)
        {
            lowLevelEventDelegate = LowLevelEventDelegate;
            this.keyStateTracker = keyStateTracker ?? throw new ArgumentNullException(nameof(keyStateTracker));
            this.nativeHookHandle = Hook();
        }

        private HookSafeHandle Hook()
        {
            using Process currentProcess = Process.GetCurrentProcess();
            using ProcessModule currentModule = currentProcess.MainModule ?? throw new Exception("Main Module is null, cannot hook!");
            return Pinvoke.SetWindowsHookEx(Pinvoke.WH_KEYBOARD_LL, lowLevelEventDelegate, Pinvoke.GetModuleHandle(currentModule.ModuleName ?? throw new Exception("Module Name is null, cannot hook!")), 0);
        }

        private IntPtr LowLevelEventDelegate(int nCode, KeyboardMessage keyboardMessage, IntPtr keyboardDataPtr)
        {
            if (nCode >= 0)
            {
                if (keyboardMessage == KeyboardMessage.KeyUp || keyboardMessage == KeyboardMessage.SystemKeyUp)
                {
                    var keyboardData = Marshal.PtrToStructure<KeyboardData>(keyboardDataPtr) ?? throw new Exception("Failed to read KeyboardData struct.");
                    keyStateTracker.PutKeyState(keyboardData.vkCode, KeyState.Up);
                    KeyUp?.Invoke(this, new KeyboardEventArgs(keyboardData, keyStateTracker));
                }

                if (keyboardMessage == KeyboardMessage.KeyDown || keyboardMessage == KeyboardMessage.SystemKeyDown)
                {
                    var keyboardData = Marshal.PtrToStructure<KeyboardData>(keyboardDataPtr) ?? throw new Exception("Failed to read KeyboardData struct.");

                    if (keyStateTracker.CheckKeyState(keyboardData.vkCode, KeyState.Up)
                        || keyStateTracker.CheckKeyState(keyboardData.vkCode, KeyState.Unknown))
                    {
                        keyStateTracker.PutKeyState(keyboardData.vkCode, KeyState.Down);
                        KeyDown?.Invoke(this, new KeyboardEventArgs(keyboardData, keyStateTracker));
                    }
                    else
                    {
                        keyStateTracker.PutKeyState(keyboardData.vkCode, KeyState.Held);
                        KeyHeld?.Invoke(this, new KeyboardEventArgs(keyboardData, keyStateTracker));
                    }
                }
            }

            return Pinvoke.CallNextHookEx(nativeHookHandle, nCode, (IntPtr)keyboardMessage, keyboardDataPtr);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (nativeHookHandle != null && !nativeHookHandle.IsInvalid)
            {
                nativeHookHandle.Dispose();
            }
        }
    }
}
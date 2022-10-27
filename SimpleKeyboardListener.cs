using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KeyShark
{
    public class SimpleKeyboardListener : IKeyboardListener
    {
        public bool IsListening { get; private set; }

        public event EventHandler<KeyboardEventArgs>? KeyUp;
        public event EventHandler<KeyboardEventArgs>? KeyDown;
        public event EventHandler<KeyboardEventArgs>? KeyHeld;

        private IntPtr nativeHookHandle;
        private readonly Pinvoke.LowLevelKeyboardProc lowLevelEventDelegate;
        private readonly IKeyStateTracker keyStateTracker;

        public SimpleKeyboardListener(IKeyStateTracker keyStateTracker)
        {
            lowLevelEventDelegate = LowLevelEventDelegate;
            nativeHookHandle = IntPtr.Zero;
            this.keyStateTracker = keyStateTracker ?? throw new ArgumentNullException(nameof(keyStateTracker));
            IsListening = false;
        }

        ~SimpleKeyboardListener()
        {
            Stop();
        }

        public void Start()
        {
            if (IsListening)
                return;

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule ?? throw new Exception("Main Module is null, cannot hook!"))
            {
                nativeHookHandle = Pinvoke.SetWindowsHookEx(Pinvoke.WH_KEYBOARD_LL, lowLevelEventDelegate, Pinvoke.GetModuleHandle(curModule.ModuleName ?? throw new Exception("Module Name is null, cannot hook!")), 0);
                IsListening = true;
            }
        }

        public void Stop()
        {
            if (!IsListening)
                return;

            Pinvoke.UnhookWindowsHookEx(nativeHookHandle);
            nativeHookHandle = IntPtr.Zero;
            IsListening = false;
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

    }
}
namespace KeyShark
{
    public class KeyboardEventArgs
    {
        public KeyboardData KeyboardData { get; private set; }
        public VKey KeyCode => KeyboardData.vkCode;
        public KeyState KeyState => KeyStateTracker.GetKeyState(KeyCode);
        public IKeyStateTracker KeyStateTracker { get; private set; }

        public bool ShiftDown => LeftShiftDown || RightShiftDown;
        public bool CtrlDown => LeftCtrlDown || RightCtrlDown;
        public bool AltDown => LeftAltDown || RightAltDown;
        public bool WinDown => LeftWindowsDown || RightWindowsDown;

        public bool LeftShiftDown => KeyStateTracker.CheckKeyState(VKey.LSHIFT, KeyState.Down) || KeyStateTracker.CheckKeyState(VKey.LSHIFT, KeyState.Held);
        public bool RightShiftDown => KeyStateTracker.CheckKeyState(VKey.RSHIFT, KeyState.Down) || KeyStateTracker.CheckKeyState(VKey.RSHIFT, KeyState.Held);
        public bool LeftCtrlDown => KeyStateTracker.CheckKeyState(VKey.LCONTROL, KeyState.Down) || KeyStateTracker.CheckKeyState(VKey.LCONTROL, KeyState.Held);
        public bool RightCtrlDown => KeyStateTracker.CheckKeyState(VKey.RCONTROL, KeyState.Down) || KeyStateTracker.CheckKeyState(VKey.RCONTROL, KeyState.Held);
        public bool LeftAltDown => KeyStateTracker.CheckKeyState(VKey.LMENU, KeyState.Down) || KeyStateTracker.CheckKeyState(VKey.LMENU, KeyState.Held);
        public bool RightAltDown => KeyStateTracker.CheckKeyState(VKey.RMENU, KeyState.Down) || KeyStateTracker.CheckKeyState(VKey.RMENU, KeyState.Held);
        public bool LeftWindowsDown => KeyStateTracker.CheckKeyState(VKey.LWIN, KeyState.Down) || KeyStateTracker.CheckKeyState(VKey.LWIN, KeyState.Held);
        public bool RightWindowsDown => KeyStateTracker.CheckKeyState(VKey.RWIN, KeyState.Down) || KeyStateTracker.CheckKeyState(VKey.RWIN, KeyState.Held);

        public KeyboardEventArgs(KeyboardData keyboardData, IKeyStateTracker keyStateTracker)
        {
            KeyboardData = keyboardData;
            KeyStateTracker = keyStateTracker ?? throw new ArgumentNullException(nameof(keyStateTracker));
        }
    }
}
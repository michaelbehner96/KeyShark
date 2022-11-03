using KeyShark.Native;
using System.Reflection.Metadata.Ecma335;

namespace KeyShark
{
    public class Keybind
    {
        public VKey[]? KeyCodes { get; set; }

        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;

                if (!_enabled)
                    recentlyPressed = false;
            }
        }

        public event EventHandler? KeybindPressed;
        public event EventHandler? KeybindHeld;
        public event EventHandler? KeybindReleased;

        private bool recentlyPressed;
        private readonly IKeyboardListener keyboardListener;

        public Keybind(IKeyboardListener keyboardListener, params VKey[]? keyCodes)
        {
            KeyCodes = keyCodes;
            this.keyboardListener = keyboardListener ?? throw new ArgumentNullException(nameof(keyboardListener));
            this.keyboardListener.KeyUp += KeyboardListener_KeyUp;
            this.keyboardListener.KeyHeld += KeyboardListener_KeyHeld;
            this.keyboardListener.KeyDown += KeyboardListener_KeyDown;
            Enabled = true;
            recentlyPressed = false;
        }

        private void KeyboardListener_KeyDown(object? sender, KeyboardEventArgs e)
        {
            if (Enabled && KeybindCombinationIsPressed(e.KeyStateTracker))
            {
                recentlyPressed = true;
                KeybindPressed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void KeyboardListener_KeyHeld(object? sender, KeyboardEventArgs e)
        {
            if (Enabled && KeybindCombinationIsPressed(e.KeyStateTracker))
            {
                if (recentlyPressed)
                    KeybindHeld?.Invoke(this, EventArgs.Empty);
            }
        }

        private void KeyboardListener_KeyUp(object? sender, KeyboardEventArgs e)
        {
            if (Enabled && !KeybindCombinationIsPressed(e.KeyStateTracker))
            {
                if (recentlyPressed)
                {
                    recentlyPressed = false;
                    KeybindReleased?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private bool KeybindCombinationIsPressed(IKeyStateTracker keyStateTracker)
        {
            if (KeyCodes == null || KeyCodes.Length == 0) return false;

            var result = true;

            foreach (var keyCode in KeyCodes)
            {
                if (!(keyStateTracker.CheckKeyState(keyCode, KeyState.Down) || keyStateTracker.CheckKeyState(keyCode, KeyState.Held)))
                    result = false;
            }

            return result;
        }
    }
}
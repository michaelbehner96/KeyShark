namespace KeyShark
{
    public class Keystroke
    {
        public VKey[] KeyCodes { get; private set; }

        public Keystroke(params VKey[] keyCodes)
        {
            KeyCodes = keyCodes ?? throw new ArgumentNullException(nameof(keyCodes));
        }

        public bool Check(IKeyStateTracker keyStateTracker)
        {
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
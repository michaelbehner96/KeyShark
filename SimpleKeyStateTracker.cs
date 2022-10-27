namespace KeyShark
{
    public class SimpleKeyStateTracker : IKeyStateTracker
    {
        public Dictionary<VKey, KeyState> KeyStates { get; private set; } = new Dictionary<VKey, KeyState>();

        public void PutKeyState(VKey keyCode, KeyState keyState)
        {
            if (KeyStates.ContainsKey(keyCode))
                KeyStates[keyCode] = keyState;
            else
                KeyStates.Add(keyCode, keyState);
        }

        public KeyState GetKeyState(VKey keyCode)
        {
            if (!KeyStates.ContainsKey(keyCode))
                return KeyState.Unknown;

            return KeyStates[keyCode];
        }

        public bool CheckKeyState(VKey keyCode, KeyState keyState)
        {
            if (!KeyStates.ContainsKey(keyCode))
                if (keyState == KeyState.Unknown)
                    return true;

            if (KeyStates.ContainsKey(keyCode))
                if (KeyStates[keyCode] == keyState)
                    return true;

            return false;
        }
    }
}
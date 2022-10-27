namespace KeyShark
{
    public interface IKeyStateTracker
    {
        public void PutKeyState(VKey keyCode, KeyState isKeyDown);
        public KeyState GetKeyState(VKey keyCode);
        public bool CheckKeyState(VKey keyCode, KeyState keyState);
    }
}
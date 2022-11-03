namespace KeyShark
{
    public interface IKeyboardListener : IDisposable
    {
        public event EventHandler<KeyboardEventArgs>? KeyUp;
        public event EventHandler<KeyboardEventArgs>? KeyDown;
        public event EventHandler<KeyboardEventArgs>? KeyHeld;
    }
}
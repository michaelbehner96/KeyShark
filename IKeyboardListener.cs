namespace KeyShark
{
    public interface IKeyboardListener : IDisposable
    {
        public bool IsListening { get; }

        public event EventHandler<KeyboardEventArgs>? KeyUp;
        public event EventHandler<KeyboardEventArgs>? KeyDown;
        public event EventHandler<KeyboardEventArgs>? KeyHeld;

        public void Resume();

        public void Pause();
    }
}
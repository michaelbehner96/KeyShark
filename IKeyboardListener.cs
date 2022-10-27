namespace KeyShark
{
    public interface IKeyboardListener
    {
        public event EventHandler<KeyboardEventArgs>? KeyUp;
        public event EventHandler<KeyboardEventArgs>? KeyDown;
        public event EventHandler<KeyboardEventArgs>? KeyHeld;

        public void Start();

        public void Stop();
    }
}
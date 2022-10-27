namespace KeyShark.Native
{
    [Flags]
    public enum KeyboardDataFlags : uint
    {
        Extended = 0x01,
        Injected = 0x10,
        AltDown = 0x20,
        Up = 0x80,
    }
}
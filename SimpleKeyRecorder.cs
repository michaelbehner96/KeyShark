using KeyShark.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeyShark
{
    public class SimpleKeyRecorder : IKeyRecorder
    {
        public bool IsRecording { get; private set; }

        private CancellationTokenSource? cancellationTokenSource;
        private readonly VKey[] auxillaryKeys;
        private readonly IKeyboardListener keyboardListener;
        private readonly IKeyStateTracker keyStateTracker;

        public SimpleKeyRecorder(IKeyboardListener keyboardListener, IKeyStateTracker keyStateTracker, params VKey[] auxillaryKeys)
        {
            this.keyboardListener = keyboardListener ?? throw new ArgumentNullException(nameof(keyboardListener));
            this.keyStateTracker = keyStateTracker ?? throw new ArgumentNullException(nameof(keyStateTracker));
            this.auxillaryKeys = auxillaryKeys ?? throw new ArgumentNullException(nameof(auxillaryKeys));

            this.keyboardListener.KeyDown += OnKeyDown;
            this.keyboardListener.KeyUp += OnKeyUp;
        }

        private void OnKeyUp(object? sender, KeyboardEventArgs e)
        {
            if (!IsRecording)
                return;

            keyStateTracker.PutKeyState(e.KeyCode, KeyState.Up);
        }

        private void OnKeyDown(object? sender, KeyboardEventArgs e)
        {
            if (!IsRecording)
                return;

            keyStateTracker.PutKeyState(e.KeyCode, KeyState.Down);

            if (!auxillaryKeys.Contains(e.KeyCode))
                IsRecording = false;
        }

        public async Task<VKey[]?> RecordKeysAsync()
        {
            if (IsRecording) throw new Exception("Recorder already recording keys...");

            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            IsRecording = true;

            try
            {
                await Task.Run(() =>
                {
                    while (IsRecording)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }, cancellationToken);

                return keyStateTracker.GetKeysInState(KeyState.Down);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            finally
            {
                Reset();
            }
        }

        public void CancelRecording()
        {
            if (IsRecording)
                cancellationTokenSource?.Cancel();
        }

        private void Reset()
        {
            IsRecording = false;
            keyStateTracker.ClearAllStates();
            cancellationTokenSource?.Dispose();
        }
    }














    public class Deb
    {
        public Deb()
        {


        }
    }

    public interface IActor<TActionContext> where TActionContext : notnull
    {
        public ReadOnlyDictionary<TActionContext, IActionRelay> GetActions();
        public void RegisterContext(TActionContext context, IActionRelay action);
        public void UnregisterContext(TActionContext context);
        public void Act(TActionContext context);
    }

    public class ConsoleContext
    {
        public string Message { get; set; }

    }

    public interface IActionRelay
    {
        public Action? Relay { get; set; }
    }

    public class KeybindActionRelay : IActionRelay
    {
        [Flags]
        public enum KeybindActionType
        {
            Released = 1, Pressed = 2, Held = 4
        }

        public Action? Relay { get; set; }
        public Keybind Keybind { get; private set; }

        public KeybindActionRelay(Keybind keybind, KeybindActionType keybindActionType)
        {
            Keybind = keybind;
            if (keybindActionType.HasFlag(KeybindActionType.Released))
                Keybind.KeybindReleased += (sender, args) => { Relay?.Invoke(); };
            if (keybindActionType.HasFlag(KeybindActionType.Held))
                Keybind.KeybindHeld += (sender, args) => { Relay?.Invoke(); };
            if (keybindActionType.HasFlag(KeybindActionType.Pressed))
                Keybind.KeybindPressed += (sender, args) => { Relay?.Invoke(); };
        }
    }

    public class ConsoleActor : IActor<ConsoleContext>
    {
        private Dictionary<ConsoleContext, IActionRelay> actions;

        public ConsoleActor()
        {
            actions = new Dictionary<ConsoleContext, IActionRelay>();
        }

        public void Act(ConsoleContext context)
        {
            Console.WriteLine(context.Message);
        }

        public ReadOnlyDictionary<ConsoleContext, IActionRelay> GetActions()
        {
            return new ReadOnlyDictionary<ConsoleContext, IActionRelay>(actions);
        }

        public void RegisterContext(ConsoleContext context, IActionRelay action)
        {
            action.Relay = new Action(() => Act(context));
            actions.Add(context, action);
        }

        public void UnregisterContext(ConsoleContext context)
        {
            if (actions.ContainsKey(context))
            {
                actions[context].Relay = null;
                actions.Remove(context);
            }
        }
    }

}

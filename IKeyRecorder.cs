using KeyShark.Native;
using System.Threading;
using System.Threading.Tasks;

namespace KeyShark
{
    public interface IKeyRecorder
    {
        public bool IsRecording { get; }
        public Task<VKey[]?> RecordKeysAsync();
        public void CancelRecording();
    }
}

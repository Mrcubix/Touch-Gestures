using System.Threading.Tasks;
using OpenTabletDriver.External.Common.Contracts;

namespace TouchGestures.Lib.Contracts
{
    public interface IGesturesDaemon : IPluginDaemon
    {
        public Task<bool> StartRecording();
        public Task<bool> StopRecording();
    }
}
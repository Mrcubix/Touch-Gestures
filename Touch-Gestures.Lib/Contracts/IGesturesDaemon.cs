using System.Threading.Tasks;
using OpenTabletDriver.External.Common.Contracts;
using TouchGestures.Lib.Entities;

namespace TouchGestures.Lib.Contracts
{
    public interface IGesturesDaemon : IPluginDaemon
    {
        public Task<SerializableSettings> GetSettings();
        public Task<bool> UpdateSettings(SerializableSettings settings);
        public Task<bool> StartRecording();
        public Task<bool> StopRecording();
    }
}
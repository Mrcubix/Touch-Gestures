using System.Numerics;
using System.Threading.Tasks;
using OpenTabletDriver.External.Common.Contracts;
using TouchGestures.Lib.Entities;

namespace TouchGestures.Lib.Contracts
{
    public interface IGesturesDaemon : IPluginDaemon
    {
        public Task<bool> IsTabletConnected();
        public Task<Vector2> GetTabletSize();
        public Task<float> GetTabletLinesPerMM();
        public Task<SerializableSettings> GetSettings();
        public Task<bool> SaveSettings();
        public Task<bool> UpdateSettings(SerializableSettings settings);
        public Task<bool> StartRecording();
        public Task<bool> StopRecording();
    }
}
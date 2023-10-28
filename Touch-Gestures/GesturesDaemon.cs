using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Lib.Contracts;

namespace TouchGestures
{
    public class GesturesDaemon : IGesturesDaemon
    {
        public bool IsReady { get; private set; }
        
        public Task<List<SerializablePlugin>> GetPlugins()
        {
            return Task.FromResult(new List<SerializablePlugin>());
        }

        public Task<bool> StartRecording()
        {
            return Task.FromResult(true);
        }

        public Task<bool> StopRecording()
        {
            return Task.FromResult(true);
        }
    }
}
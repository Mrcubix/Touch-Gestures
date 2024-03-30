using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using OpenTabletDriver.External.Common.Contracts;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Lib.Contracts
{
    public interface IGesturesDaemon : IPluginDaemon
    {
        event EventHandler<IEnumerable<SharedTabletReference>> TabletsChanged;

        public Task<bool> IsTabletConnected();
        public Task<SharedTabletReference[]> GetTablets();
        
        [Obsolete("Use GetTablets() and fetch the TouchDigitizer from the returned SharedTabletReferences instead.")]
        public Task<Vector2> GetTabletSize();

        [Obsolete("Use GetTablets() and fetch the TouchDigitizer from the returned SharedTabletReferences instead.")]
        public Task<Vector2> GetTabletLinesPerMM();
        public Task<SerializableSettings> GetSettings();
        public Task<bool> SaveSettings();
        public Task<bool> UpdateSettings(SerializableSettings settings);
        public Task<bool> UpdateProfile(SerializableProfile profile);
        public Task<bool> StartRecording();
        public Task<bool> StopRecording();
    }
}
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

        /// <summary>
        ///   Returns whether a tablet is connected.
        /// </summary>
        /// <returns>True if a tablet is connected, false otherwise.</returns>
        public Task<bool> IsTabletConnected();

        /// <summary>
        ///   Returns the connected tablets.
        /// </summary>
        public Task<IEnumerable<SharedTabletReference>> GetTablets();
        
        /// <summary>
        ///   Returns the tablet size.
        /// </summary>
        /// <remarks>
        ///   This method is obsolete.
        /// </remarks>
        [Obsolete("Use GetTablets() and fetch the TouchDigitizer from the returned SharedTabletReferences instead.")]
        public Task<Vector2> GetTabletSize();

        /// <summary>
        ///   Returns the tablet lines per mm.
        /// </summary>
        /// <remarks>
        ///   This method is obsolete.
        /// </remarks>
        [Obsolete("Use GetTablets() and fetch the TouchDigitizer from the returned SharedTabletReferences instead.")]
        public Task<Vector2> GetTabletLinesPerMM();

        /// <summary>
        ///   Request settings in serializable form.
        /// </summary>
        public Task<SerializableSettings> GetSettings();

        /// <summary>
        ///   Save settings.
        /// </summary>
        /// <returns>True if the settings were saved successfully, false otherwise.</returns>
        public Task<bool> SaveSettings();

        /// <summary>
        ///   Update All settings.
        /// </summary>
        public Task<bool> UpdateSettings(SerializableSettings settings);

        /// <summary>
        ///   Update a specific profile.
        /// </summary>
        /// <param name="profile">The profile to update.</param>
        /// <returns>True if the profile was updated successfully, false otherwise.</returns>
        public Task<bool> UpdateProfile(SerializableProfile profile);

        /// <summary>
        ///   Start recording (Used for future node-based gesture creation).
        /// </summary>
        public Task<bool> StartRecording();

        /// <summary>
        ///   Stop recording (Used for future node-based gesture creation).
        /// </summary>
        public Task<bool> StopRecording();
    }
}
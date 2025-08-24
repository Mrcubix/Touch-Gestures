using OpenTabletDriver.External.Common.Serializables;

namespace TouchGestures.Lib.Interfaces
{
    public interface ISerializable
    {
        /// <summary>
        ///    The serializable store containing plugin settings for the UX to use.
        /// </summary>
        public SerializablePluginSettingsStore? Store { get; set; }
    }
}
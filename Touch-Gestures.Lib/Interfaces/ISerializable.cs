using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin;

namespace TouchGestures.Lib.Interfaces
{
    public interface ISerializable
    {
        /// <summary>
        ///  The gesture's property, used to store the gesture's settings.
        /// </summary>
        public SerializablePluginSettings? PluginProperty { get; set; }
    }
}
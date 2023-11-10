using OpenTabletDriver.External.Common.Serializables;

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
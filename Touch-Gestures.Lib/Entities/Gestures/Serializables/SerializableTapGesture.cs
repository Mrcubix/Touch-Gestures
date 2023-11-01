using System.Numerics;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Entities.Gestures;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Serializables.Gestures
{
    public class SerializableTapGesture : TapGesture, ISerializable
    {
        #region Constructors

        public SerializableTapGesture() : base()
        {
        }

        public SerializableTapGesture(Vector2 threshold) : base(threshold)
        {
        }

        public SerializableTapGesture(Vector2 threshold, double deadline) : base(threshold, deadline)
        {
        }

        public SerializableTapGesture(Vector2 threshold, double deadline, int requiredTouchesCount) : base(threshold, deadline, requiredTouchesCount)
        {
        }

        #endregion

        /// <summary>
        ///   The Serializable representation of the plugin property.
        /// </summary>
        public SerializablePluginSettings? PluginProperty { get; set; }
    }
}
using System.Drawing;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Entities.Gestures;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Serializables.Gestures
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializableTapGesture : TapGesture, ISerializable, INamed
    {
        #region Constructors

        public SerializableTapGesture() : base()
        {
        }

        public SerializableTapGesture(Rectangle bounds) : base(bounds)
        {
        }

        public SerializableTapGesture(Rectangle bounds, double deadline) : base(bounds, deadline)
        {
        }

        public SerializableTapGesture(Rectangle bounds, double deadline, int requiredTouchesCount) : base(bounds, deadline, requiredTouchesCount)
        {
        }

        public SerializableTapGesture(TapGesture gesture) : base(gesture.Bounds, gesture.Deadline, gesture.RequiredTouchesCount)
        {
        }

        #endregion

        /// <summary>
        ///   The Serializable representation of the plugin property.
        /// </summary>
        [JsonProperty]
        public SerializablePluginSettings? PluginProperty { get; set; }

        public string Name => $"{RequiredTouchesCount}-Touch Tap";
    }
}
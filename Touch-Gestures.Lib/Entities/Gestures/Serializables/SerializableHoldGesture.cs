using System.Drawing;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Serializables.Gestures
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializableHoldGesture : HoldGesture, ISerializable, INamed
    {
        #region Constructors

        public SerializableHoldGesture() : base()
        {
        }

        public SerializableHoldGesture(Rectangle bounds) : base(bounds)
        {
        }

        public SerializableHoldGesture(Rectangle bounds, double deadline) : base(bounds, deadline)
        {
        }

        public SerializableHoldGesture(Rectangle bounds, double deadline, int requiredTouchesCount) : base(bounds, deadline, requiredTouchesCount)
        {
        }

        public SerializableHoldGesture(TapGesture gesture) : base(gesture.Bounds, gesture.Threshold, gesture.Deadline, gesture.RequiredTouchesCount)
        {
        }

        #endregion

        /// <summary>
        ///   The Serializable representation of the plugin property.
        /// </summary>
        [JsonProperty]
        public SerializablePluginSettings? PluginProperty { get; set; }

        public string Name => $"{RequiredTouchesCount}-Touch Hold";
    }
}
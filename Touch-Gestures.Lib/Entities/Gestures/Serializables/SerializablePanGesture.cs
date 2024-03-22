using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Entities.Gestures;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Serializables.Gestures
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializablePanGesture : PanGesture, ISerializable, INamed
    {
        #region Constructors

        public SerializablePanGesture() : base()
        {
        }

        public SerializablePanGesture(Vector2 threshold) : base(threshold)
        {
        }

        public SerializablePanGesture(double deadline) : base(deadline)
        {
        }

        public SerializablePanGesture(Vector2 threshold, double deadline) : base(threshold, deadline)
        {
        }

        public SerializablePanGesture(SwipeDirection direction) : base(direction)
        {
        }

        public SerializablePanGesture(Vector2 threshold, double deadline, SwipeDirection direction, SharedArea area) 
            : base(threshold, deadline, direction, area)
        {
        }

        public SerializablePanGesture(SwipeGesture gesture) 
            : base(gesture.Threshold, gesture.Deadline, gesture.Direction, gesture.Bounds)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The Serializable representation of the plugin property.
        /// </summary>
        [JsonProperty]
        public SerializablePluginSettings? PluginProperty { get; set; }

        public string Name => $"{Direction} Pan";

        #endregion
    }
}
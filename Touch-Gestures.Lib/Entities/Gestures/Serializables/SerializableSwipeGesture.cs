using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Entities.Gestures;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Serializables.Gestures
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializableSwipeGesture : SwipeGesture, ISerializable, INamed
    {
        #region Constructors

        public SerializableSwipeGesture() : base()
        {
        }

        public SerializableSwipeGesture(Vector2 threshold) : base(threshold)
        {
        }

        public SerializableSwipeGesture(double deadline) : base(deadline)
        {
        }

        public SerializableSwipeGesture(Vector2 threshold, double deadline) : base(threshold, deadline)
        {
        }

        public SerializableSwipeGesture(SwipeDirection direction) : base(direction)
        {
        }

        public SerializableSwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction, SharedArea area) 
            : base(threshold, deadline, direction, area)
        {
        }

        public SerializableSwipeGesture(SwipeGesture gesture) 
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

        public string Name => $"{Direction} Swipe";

        #endregion
    }
}
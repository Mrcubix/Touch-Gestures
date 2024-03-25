using System.Drawing;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Serializables.Gestures
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializablePinchGesture : PinchGesture, ISerializable, INamed
    {
        #region Constructors

        public SerializablePinchGesture() : base()
        {
        }

        public SerializablePinchGesture(SharedArea? bounds) : base(bounds)
        {
        }

        public SerializablePinchGesture(Rectangle bounds) : base(bounds)
        {
        }

        public SerializablePinchGesture(PinchGesture pinchGesture) : this(pinchGesture.Bounds)
        {
            DistanceThreshold = pinchGesture.DistanceThreshold;
            AngleThreshold = pinchGesture.AngleThreshold;
            IsInner = pinchGesture.IsInner;
            IsClockwise = pinchGesture.IsClockwise;
        }

        public SerializablePinchGesture(SharedArea? bounds, SerializablePluginSettings pluginProperty) : base(bounds)
        {
            PluginProperty = pluginProperty;
        }

        public SerializablePinchGesture(Rectangle bounds, SerializablePluginSettings pluginProperty) : base(bounds)
        {
            PluginProperty = pluginProperty;
        }

        public SerializablePinchGesture(double distanceThreshold, double angleThreshold, bool isInner, bool isClockwise,
                                        SharedArea? bounds, SerializablePluginSettings pluginProperty) 
            : base(distanceThreshold, angleThreshold, isInner, isClockwise, bounds)
        {
            PluginProperty = pluginProperty;
        }

        public SerializablePinchGesture(double distanceThreshold, double angleThreshold, bool isInner, bool isClockwise,
                                        Rectangle bounds, SerializablePluginSettings pluginProperty) 
            : base(distanceThreshold, angleThreshold, isInner, isClockwise, bounds)
        {
            PluginProperty = pluginProperty;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The Serializable representation of the plugin property.
        /// </summary>
        [JsonProperty]
        public SerializablePluginSettings? PluginProperty { get; set; }

        public string Name
        {
            get
            {
                if (AngleThreshold == 0)
                {
                    var direction = IsInner ? "Inner" : "Outer";
                    return $"{direction} Pinch";
                }
                else
                {
                    var direction = IsClockwise ? "Clockwise" : "Counter-Clockwise";
                    return $"{direction} Rotation";
                }
            }
        }

        #endregion
    }
}
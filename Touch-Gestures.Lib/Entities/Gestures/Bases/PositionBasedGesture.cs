using System;
using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class PositionBasedGesture : Gesture, IPositionBasedGesture
    {
        /// <inheritdoc />
        public abstract Vector2 StartPosition { get; protected set; }

        /// <inheritdoc />
        [JsonProperty]
        public abstract Vector2 Threshold { get; set; }
    }
}
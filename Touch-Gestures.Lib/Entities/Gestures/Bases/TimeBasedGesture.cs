using System;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class TimeBasedGesture : Gesture, ITimeBasedGesture
    {
        /// <inheritdoc />
        public abstract DateTime TimeStarted { get; protected set; }

        /// <inheritdoc />
        [JsonProperty]
        public abstract double Deadline { get; protected set; }
    }
}
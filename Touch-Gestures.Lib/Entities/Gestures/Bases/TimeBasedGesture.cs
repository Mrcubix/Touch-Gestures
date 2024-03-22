using System;
using Newtonsoft.Json;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class TimeBasedGesture : Gesture, ITimeBasedGesture
    {
        /// <inheritdoc />
        public virtual DateTime TimeStarted { get; protected set; }

        /// <inheritdoc />
        [JsonProperty]
        public abstract double Deadline { get; set; }
    }
}
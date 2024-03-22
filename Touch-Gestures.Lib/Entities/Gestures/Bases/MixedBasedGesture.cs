using System;
using System.Numerics;
using Newtonsoft.Json;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class MixedBasedGesture : Gesture, IMixedBasedGesture
    {
        #region Constructors

        public MixedBasedGesture()
        {
        }

        public MixedBasedGesture(Vector2 threshold)
        {
            Threshold = threshold;
        }

        public MixedBasedGesture(double deadline)
        {
            Deadline = deadline;
        }

        public MixedBasedGesture(Vector2 threshold, double deadline)
        {
            Threshold = threshold;
            Deadline = deadline;
        }

        #endregion

        #region Properties

        public virtual Vector2 StartPosition { get; protected set; }

        public virtual DateTime TimeStarted { get; protected set; }

        [JsonProperty]
        public abstract Vector2 Threshold { get; set; }

        [JsonProperty]
        public abstract double Deadline { get; set; }

        #endregion
    }
}
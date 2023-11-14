using System;
using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class MixedBasedGesture : Gesture, IMixedBasedGesture
    {
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

        #region Properties

        public Vector2 StartPosition { get; protected set; }

        public DateTime TimeStarted { get; protected set; }

        [JsonProperty]
        public Vector2 Threshold { get; set; }

        [JsonProperty]
        public double Deadline { get; set; }

        #endregion
    }
}
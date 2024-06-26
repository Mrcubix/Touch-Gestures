using System.Numerics;
using Newtonsoft.Json;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class PositionBasedGesture : Gesture, IPositionBasedGesture
    {
        /// <inheritdoc />
        public virtual Vector2 StartPosition { get; protected set; }

        /// <inheritdoc />
        [JsonProperty]
        public abstract Vector2 Threshold { get; set; }
    }
}
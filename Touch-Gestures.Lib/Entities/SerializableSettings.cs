using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Serializables.Gestures;

namespace TouchGestures.Lib.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializableSettings : IEnumerable<Gesture>
    {
        #region Constructors

        public SerializableSettings()
        {
        }

        #endregion

        #region Properties

        [JsonProperty]
        public List<SerializableTapGesture> TapGestures { get; set; } = new();

        [JsonProperty]
        public List<SerializableHoldGesture> HoldGestures { get; set; } = new();

        [JsonProperty]
        public List<SerializableSwipeGesture> SwipeGestures { get; set; } = new();

        [JsonProperty]
        public List<SerializablePanGesture> PanGestures { get; set; } = new();

        [JsonProperty]
        public List<SerializablePinchGesture> PinchGestures { get; set; } = new();

        [JsonProperty]
        public List<SerializablePinchGesture> RotateGestures { get; set; } = new();

        #endregion

        #region IEnumerable Implementation

        /// <returns>Returns an enumerator where all gestures are aggregated.</returns>
        public IEnumerator<Gesture> GetEnumerator()
        {
            foreach (var tapGesture in TapGestures)
            {
                yield return tapGesture;
            }

            foreach (var holdGesture in HoldGestures)
            {
                yield return holdGesture;
            }

            foreach (var swipeGesture in SwipeGestures)
            {
                yield return swipeGesture;
            }

            foreach (var panGesture in PanGestures)
            {
                yield return panGesture;
            }

            foreach (var pinchGesture in PinchGestures)
            {
                yield return pinchGesture;
            }

            foreach (var rotateGesture in RotateGestures)
            {
                yield return rotateGesture;
            }

            // do the same for other gestures
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
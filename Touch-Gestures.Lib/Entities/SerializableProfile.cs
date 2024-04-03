using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Serializables.Gestures;

namespace TouchGestures.Lib.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializableProfile : IEnumerable<Gesture>
    {
        [JsonProperty]
        public string Name { get; set; } = string.Empty;

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

        #region Methods

        public void Add(Gesture gesture)
        {
            switch(gesture)
            {
                case SerializableTapGesture tapGesture:
                    TapGestures.Add(tapGesture);
                    break;
                case SerializableHoldGesture holdGesture:
                    HoldGestures.Add(holdGesture);
                    break;
                case SerializableSwipeGesture swipeGesture:
                    SwipeGestures.Add(swipeGesture);
                    break;
                case SerializablePanGesture panGesture:
                    PanGestures.Add(panGesture);
                    break;
                case SerializablePinchGesture pinchGesture:
                    AddPinch(pinchGesture);
                    break;
                default:
                    throw new ArgumentException("Unknown gesture type.");
            }
        }

        public void Remove(Gesture gesture)
        {
            switch(gesture)
            {
                case SerializableTapGesture tapGesture:
                    TapGestures.Remove(tapGesture);
                    break;
                case SerializableHoldGesture holdGesture:
                    HoldGestures.Remove(holdGesture);
                    break;
                case SerializableSwipeGesture swipeGesture:
                    SwipeGestures.Remove(swipeGesture);
                    break;
                case SerializablePanGesture panGesture:
                    PanGestures.Remove(panGesture);
                    break;
                case SerializablePinchGesture pinchGesture:
                    RemovePinch(pinchGesture);
                    break;
                default:
                    throw new ArgumentException("Unknown gesture type.");
            }
        }

        private void AddPinch(SerializablePinchGesture pinchGesture)
        {
            if (pinchGesture.DistanceThreshold > 0)
                PinchGestures.Add(pinchGesture);
            else
                RotateGestures.Add(pinchGesture);
        }

        private void RemovePinch(SerializablePinchGesture pinchGesture)
        {
            if (pinchGesture.DistanceThreshold > 0)
                PinchGestures.Remove(pinchGesture);
            else
                RotateGestures.Remove(pinchGesture);
        }

        #endregion

        #region IEnumerable Implementation

        /// <returns>Returns an enumerator where all gestures are aggregated.</returns>
        public IEnumerator<Gesture> GetEnumerator()
        {
            foreach (var tapGesture in TapGestures)
                yield return tapGesture;

            foreach (var holdGesture in HoldGestures)
                yield return holdGesture;

            foreach (var swipeGesture in SwipeGestures)
                yield return swipeGesture;

            foreach (var panGesture in PanGestures)
                yield return panGesture;

            foreach (var pinchGesture in PinchGestures)
                yield return pinchGesture;

            foreach (var rotateGesture in RotateGestures)
                yield return rotateGesture;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
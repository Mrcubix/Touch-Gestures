using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GestureProfile : IGesturesProfile
    {
        public event EventHandler? ProfileChanged;

        #region Properties

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        [JsonProperty]
        public bool IsMultiTouch { get; set; } = true;

        [JsonProperty]
        public List<TapGesture> TapGestures { get; set; } = new();

        [JsonProperty]
        public List<HoldGesture> HoldGestures { get; set; } = new();

        [JsonProperty]
        public List<SwipeGesture> SwipeGestures { get; set; } = new();

        [JsonProperty]
        public List<PanGesture> PanGestures { get; set; } = new();

        [JsonProperty]
        public List<PinchGesture> PinchGestures { get; set; } = new();

        [JsonProperty]
        public List<PinchGesture> RotateGestures { get; set; } = new();

        #endregion

        #region Methods

        /// <summary>
        ///   Constructs the bindings for this profile using a set builder.
        /// </summary>
        /// <param name="tablet">The Tablet owning these bindings.</param>
        /// <remarks>
        ///   TODO: Apply abstraction to bindings so that we use inherited classes or builders Instead of <see cref="BindingBuilder"/>.
        /// </remarks>
        public virtual void ConstructBindings(SharedTabletReference tablet)
        {
            foreach (var gesture in this)
                gesture.Binding?.Construct();
        }

        public void Update(GestureProfile profile, SharedTabletReference tablet)
        {
            UpdateFromInstance(profile, tablet);
            UpdateLPMM(tablet);

            ProfileChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateLPMM(SharedTabletReference tablet)
        {
            Vector2? lpmm = IsMultiTouch ? tablet.TouchDigitizer?.GetLPMM() :
                                           tablet.PenDigitizer?.GetLPMM();

            if (lpmm != null && lpmm != Vector2.Zero)
                foreach (var gesture in this)
                    gesture.LinesPerMM = (Vector2)lpmm;
        }

        private void UpdateFromInstance(GestureProfile profile, SharedTabletReference tablet)
        {
            var result = this ?? new GestureProfile();
            result.Name = profile.Name;
            result.IsMultiTouch = profile.IsMultiTouch;

            Clear();

            foreach (var gesture in profile)
                result.Add(gesture);

            result.ConstructBindings(tablet);
        }

        #region Collection Methods

        public void Add(Gesture gesture)
        {
            switch (gesture)
            {
                case HoldGesture holdGesture:
                    HoldGestures.Add(holdGesture);
                    break;
                case TapGesture tapGesture:
                    TapGestures.Add(tapGesture);
                    break;
                case PanGesture panGesture:
                    PanGestures.Add(panGesture);
                    break;
                case SwipeGesture swipeGesture:
                    SwipeGestures.Add(swipeGesture);
                    break;
                case PinchGesture pinchGesture:
                    AddPinch(pinchGesture);
                    break;
                default:
                    throw new ArgumentException("Unknown gesture type.");
            }
        }

        public void Remove(Gesture gesture)
        {
            switch (gesture)
            {
                case HoldGesture holdGesture:
                    HoldGestures.Remove(holdGesture);
                    break;
                case TapGesture tapGesture:
                    TapGestures.Remove(tapGesture);
                    break;
                case PanGesture panGesture:
                    PanGestures.Remove(panGesture);
                    break;
                case SwipeGesture swipeGesture:
                    SwipeGestures.Remove(swipeGesture);
                    break;
                case PinchGesture pinchGesture:
                    RemovePinch(pinchGesture);
                    break;
                default:
                    throw new ArgumentException("Unknown gesture type.");
            }
        }

        public void Clear()
        {
            TapGestures.Clear();
            HoldGestures.Clear();
            SwipeGestures.Clear();
            PanGestures.Clear();
            PinchGestures.Clear();
            RotateGestures.Clear();
        }

        private void AddPinch(PinchGesture pinchGesture)
        {
            if (pinchGesture.DistanceThreshold > 0)
                PinchGestures.Add(pinchGesture);
            else
                RotateGestures.Add(pinchGesture);
        }

        private void RemovePinch(PinchGesture pinchGesture)
        {
            if (pinchGesture.DistanceThreshold > 0)
                PinchGestures.Remove(pinchGesture);
            else
                RotateGestures.Remove(pinchGesture);
        }

        #endregion

        #endregion

        #region Event Handlers

        public void OnProfileChanged()
        {
            ProfileChanged?.Invoke(this, null!);
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
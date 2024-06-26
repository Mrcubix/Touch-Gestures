using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Serializables.Gestures;

namespace TouchGestures.Lib.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BindableProfile : IEnumerable<Gesture>
    {
        public event EventHandler? ProfileChanged;

        #region Properties

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        [JsonProperty]
        public List<BindableTapGesture> TapGestures { get; set; } = new();

        [JsonProperty]
        public List<BindableHoldGesture> HoldGestures { get; set; } = new();

        [JsonProperty]
        public List<BindableSwipeGesture> SwipeGestures { get; set; } = new();

        [JsonProperty]
        public List<BindablePanGesture> PanGestures { get; set; } = new();

        [JsonProperty]
        public List<BindablePinchGesture> PinchGestures { get; set; } = new();

        [JsonProperty]
        public List<BindablePinchGesture> RotateGestures { get; set; } = new();

        #endregion

        #region Methods

        public virtual void ConstructBindings(SharedTabletReference? tablet = null)
        {
            foreach (var gesture in TapGestures)
                gesture.Binding = BindingBuilder.Build(gesture.Store, tablet) as IBinding;

            foreach (var gesture in SwipeGestures)
                gesture.Binding = BindingBuilder.Build(gesture.Store, tablet) as IBinding;

            foreach (var gesture in HoldGestures)
                gesture.Binding = BindingBuilder.Build(gesture.Store, tablet) as IBinding;

            foreach (var gesture in PanGestures)
                gesture.Binding = BindingBuilder.Build(gesture.Store, tablet) as IBinding;

            foreach (var gesture in PinchGestures)
                gesture.Binding = BindingBuilder.Build(gesture.Store, tablet) as IBinding;

            foreach (var gesture in RotateGestures)
                gesture.Binding = BindingBuilder.Build(gesture.Store, tablet) as IBinding;
        }

        public void Add(Gesture gesture)
        {
            switch(gesture)
            {
                case BindableTapGesture tapGesture:
                    TapGestures.Add(tapGesture);
                    break;
                case BindableHoldGesture holdGesture:
                    HoldGestures.Add(holdGesture);
                    break;
                case BindableSwipeGesture swipeGesture:
                    SwipeGestures.Add(swipeGesture);
                    break;
                case BindablePanGesture panGesture:
                    PanGestures.Add(panGesture);
                    break;
                case BindablePinchGesture pinchGesture:
                    AddPinch(pinchGesture);
                    break;
                default:
                    throw new ArgumentException("Unknown gesture type.");
            }
        }

        public void Update(SerializableProfile profile, SharedTabletReference tablet, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            FromSerializable(profile, identifierToPlugin, tablet, this);
            UpdateLPMM(tablet);
            ProfileChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateLPMM(SharedTabletReference tablet)
        {
            if (tablet.TouchDigitizer == null)
                return;

            foreach (var gesture in this)
                gesture.LinesPerMM = tablet.TouchDigitizer.GetLPMM();
        }

        private void AddPinch(BindablePinchGesture pinchGesture)
        {
            if (pinchGesture.DistanceThreshold > 0)
                PinchGestures.Add(pinchGesture);
            else
                RotateGestures.Add(pinchGesture);
        }

        #endregion

        #region Event Handlers

        public void OnProfileChanged()
        {
            ProfileChanged?.Invoke(this, null!);
        }

        #endregion

        #region Static Methods

        public static BindableProfile FromSerializable(SerializableProfile profile, Dictionary<int, TypeInfo> identifierToPlugin, SharedTabletReference? tablet = null, in BindableProfile? existingProfile = null)  
        {
            var result = existingProfile ?? new BindableProfile();
            result.Name = profile.Name;

            if (existingProfile != null)
            {
                result.TapGestures.Clear();
                result.HoldGestures.Clear();
                result.SwipeGestures.Clear();
                result.PanGestures.Clear();
                result.PinchGestures.Clear();
                result.RotateGestures.Clear();
            }

            foreach (var gesture in profile.TapGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                if (BindableTapGesture.FromSerializable(gesture, identifierToPlugin, tablet) is BindableTapGesture tapGesture)
                    result.TapGestures.Add(tapGesture);
            }

            foreach (var gesture in profile.HoldGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                if (BindableHoldGesture.FromSerializable(gesture, identifierToPlugin, tablet) is BindableHoldGesture holdGesture)
                    result.HoldGestures.Add(holdGesture);
            }

            foreach (var gesture in profile.SwipeGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                if (BindableSwipeGesture.FromSerializable(gesture, identifierToPlugin, tablet) is BindableSwipeGesture swipeGesture)
                    result.SwipeGestures.Add(swipeGesture);
            }

            foreach (var gesture in profile.PanGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                if (BindablePanGesture.FromSerializable(gesture, identifierToPlugin, tablet) is BindablePanGesture panGesture)
                    result.PanGestures.Add(panGesture);
            }

            foreach (var gesture in profile.PinchGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                if (BindablePinchGesture.FromSerializable(gesture, identifierToPlugin, tablet) is BindablePinchGesture pinchGesture)
                    result.PinchGestures.Add(pinchGesture);
            }

            foreach (var gesture in profile.RotateGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                if (BindablePinchGesture.FromSerializable(gesture, identifierToPlugin, tablet) is BindablePinchGesture rotateGesture)
                    result.RotateGestures.Add(rotateGesture);       
            }

            return result;
        }

        public static SerializableProfile ToSerializable(BindableProfile profile, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            var result = new SerializableProfile();
            {
                result.Name = profile.Name;
            }

            foreach (var gesture in profile.TapGestures)
                if (BindableTapGesture.ToSerializable(gesture, identifierToPlugin) is SerializableTapGesture tapGesture)
                    result.TapGestures.Add(tapGesture);

            foreach (var gesture in profile.HoldGestures)
                if (BindableHoldGesture.ToSerializable(gesture, identifierToPlugin) is SerializableHoldGesture holdGesture)
                    result.HoldGestures.Add(holdGesture);

            foreach (var gesture in profile.SwipeGestures)
                if (BindableSwipeGesture.ToSerializable(gesture, identifierToPlugin) is SerializableSwipeGesture swipeGesture)
                    result.SwipeGestures.Add(swipeGesture);

            foreach (var gesture in profile.PanGestures)
                if (BindablePanGesture.ToSerializable(gesture, identifierToPlugin) is SerializablePanGesture panGesture)
                    result.PanGestures.Add(panGesture);

            foreach (var gesture in profile.PinchGestures)
                if (BindablePinchGesture.ToSerializable(gesture, identifierToPlugin) is SerializablePinchGesture pinchGesture)
                    result.PinchGestures.Add(pinchGesture);

            foreach (var gesture in profile.RotateGestures)
                if (BindablePinchGesture.ToSerializable(gesture, identifierToPlugin) is SerializablePinchGesture rotateGesture)
                    result.RotateGestures.Add(rotateGesture);

            return result;
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
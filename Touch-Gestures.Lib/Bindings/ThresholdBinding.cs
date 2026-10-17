using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin.Tablet;

namespace TouchGestures.Lib.Bindings
{
    public abstract class ThresholdBinding : Binding
    {
        public ThresholdBinding() { }

        public ThresholdBinding(float activationThreshold, SerializablePluginSettingsStore store, Dictionary<int, TypeInfo> identifierToPlugin)
            : base(store, identifierToPlugin)
        {
            ActivationThreshold = activationThreshold;
        }

        /// <summary>
        ///   The threshold at which the binding should be activated.<br/>
        ///   Usually represented by a slider in the UI.
        /// </summary>
        public float ActivationThreshold { set; get; }

        public virtual void Invoke(IDeviceReport report, float value)
        {
            bool newState = value > ActivationThreshold;

            base.Invoke(report, newState);
        }
    }
}
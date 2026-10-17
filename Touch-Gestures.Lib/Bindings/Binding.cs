using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin.Tablet;

namespace TouchGestures.Lib.Bindings
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Binding
    {
        public Binding() { }

        public Binding(SerializablePluginSettingsStore? store, Dictionary<int, TypeInfo> identifierToPlugin)
            => FromSerializable(store, identifierToPlugin);

        public bool State { protected set; get; }

        protected bool PreviousState { set; get; }

        public abstract void Press(IDeviceReport report);
        public abstract void Release(IDeviceReport report);

        public virtual void Invoke(IDeviceReport report, bool newState)
        {
            if (newState && !PreviousState)
                Press(report);
            else if (!newState && PreviousState)
                Release(report);

            PreviousState = newState;
        }

        public abstract void Construct();

        public abstract SerializablePluginSettingsStore? ToSerializable(Dictionary<int, TypeInfo> identifierToPlugin);

        public abstract void FromSerializable(SerializablePluginSettingsStore? store, Dictionary<int, TypeInfo> identifierToPlugin);
    }
}
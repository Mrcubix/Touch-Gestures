using System.Reflection;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Lib.Bindings;
using System.Collections.Generic;
using TouchGestures.Lib.Extensions.Reflection;

namespace TouchGestures.Bindings
{
    public class StableThresholdBinding : ThresholdBinding
    {
        public StableThresholdBinding() { }

        public StableThresholdBinding(float activationThreshold, SerializablePluginSettingsStore store,
                                           Dictionary<int, TypeInfo> identifierToPlugin, TabletState? tablet = null)
            : base(activationThreshold, store, identifierToPlugin)
        {
            Tablet = tablet;
        }

        public TabletState? Tablet { get; set; }

        [JsonProperty]
        public PluginSettingStore? Store { get; set; }

        public IBinding? Binding { get; protected set; }

        public override void Construct() => Binding = Store?.Construct<IBinding>();

        public override void Press(IDeviceReport report)
        {
            Invoke(report, 1.0f);
        }

        public override void Release(IDeviceReport report)
        {
            Invoke(report, 1.0f);
        }

        public override SerializablePluginSettingsStore? ToSerializable(Dictionary<int, TypeInfo> identifierToPlugin)
        {
            return Store?.ToSerializable();
        }

        public override void FromSerializable(SerializablePluginSettingsStore? store, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            Store = store?.FromSerializable();
        }
    }
}
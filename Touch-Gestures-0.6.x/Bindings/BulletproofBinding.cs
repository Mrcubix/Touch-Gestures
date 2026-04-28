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
    public class BulletproofBinding : Binding
    {
        public BulletproofBinding() { }

        public BulletproofBinding(SerializablePluginSettingsStore? store, TabletReference? tablet = null, IServiceManager? provider = null)
            : base(store, null!)
        {
            Tablet = tablet;
            Provider = provider;
        }

        public IServiceManager? Provider { get; set; }
        public TabletReference? Tablet { get; set; }

        [JsonProperty]
        public PluginSettingStore? Store { get; set; }

        public IBinding? Binding { get; protected set; }

        public override void Construct() => Binding = Store?.Construct<IBinding>(Provider, Tablet);

        public override void Press(IDeviceReport report)
        {
            if (Binding is IStateBinding stateBinding)
                stateBinding.Press(Tablet, report);
        }

        public override void Release(IDeviceReport report)
        {
            if (Binding is IStateBinding stateBinding)
                stateBinding.Release(Tablet, report);
        }

        public override SerializablePluginSettingsStore? ToSerializable(Dictionary<int, TypeInfo> identifierToPlugin)
        {
            return Store?.ToSerializable();
        }

        public override void FromSerializable(SerializablePluginSettingsStore? store, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            Store = store?.FromSerializable();
        }

        public override string ToString() => Binding?.ToString() ?? string.Empty;
    }
}
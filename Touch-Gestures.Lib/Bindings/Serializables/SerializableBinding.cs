using System.Collections.Generic;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Enums;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.External.Common.Serializables.Properties;

namespace TouchGestures.Lib.Bindings.Serializables
{
    public class SerializableBinding : SerializablePlugin
    {
        [JsonConstructor]
        public SerializableBinding() : base()
        {
            Type = PluginType.Binding;
        }

        public SerializableBinding(string? pluginName, string? fullName, int identifier, IEnumerable<SerializableProperty> properties)
        {
            PluginName = pluginName;
            FullName = fullName;
            Identifier = identifier;
            Properties = new(properties);
        }

        public bool IsThresholdBinding { get; init; } = false;
    }
}
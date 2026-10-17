using System.Collections.Generic;
using OpenTabletDriver.External.Common.Serializables.Properties;

namespace TouchGestures.Lib.Bindings.Serializables
{
    public class SerializableThresholdBinding : SerializableBinding
    {
        public SerializableThresholdBinding() : base()
        {
            IsThresholdBinding = true;
        }

        public SerializableThresholdBinding(string? pluginName, string? fullName, int identifier, IEnumerable<SerializableProperty> properties)
        {
            PluginName = pluginName;
            FullName = fullName;
            Identifier = identifier;
            Properties = new(properties);
        }
    }
}
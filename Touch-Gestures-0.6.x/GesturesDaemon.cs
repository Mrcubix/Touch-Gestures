using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.Enums;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.External.Common.Serializables.Properties;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using TouchGestures.Converters;
using TouchGestures.Extensions.Reflection;
using TouchGestures.Lib;
using TouchGestures.Lib.Converters.Json;
using TouchGestures.Lib.Entities;

namespace TouchGestures
{
    /// <summary>
    ///   Manages settings for each tablets as well as the RPC server.
    /// </summary>
    [PluginName(PLUGIN_NAME)]
    public class GesturesDaemon : GesturesDaemonBase, ITool
    {
        #region Constructors

        public GesturesDaemon() : base()
        {
            var _profileConverter = new GestureProfileConverter();

            if (_rpcServer == null)
            {
                _rpcServer = new RpcServer<GesturesDaemonBase>("GesturesDaemon", this);
                _rpcServer.Converters.Add(new SharedAreaConverter());
                _rpcServer.Converters.Add(_profileConverter);
            }

            Settings.Converters.Add(_profileConverter);

            Instance ??= this;
        }

        #endregion

        #region Methods

        protected override void SerializePlugins()
        {
            Plugins.Clear();

            foreach (var IdentifierPluginPair in IdentifierToPluginConversion)
            {
                var plugin = IdentifierPluginPair.Value;

                var store = new PluginSettingStore(plugin);

                var type = store.GetTypeInfo();

                if (type == null)
                    continue; // type doesn't exist

                // There are situation where the name isn't specified, in which case we use the type's FullName
                var pluginName = plugin.GetCustomAttribute<PluginNameAttribute>()?.Name
                                 ?? plugin.FullName ?? $"Plugin {IdentifierPluginPair.Key}";

                // We only support properties decorated with the [Property] attribute
                var properties = from property in type.GetProperties()
                                 let attrs = property.GetCustomAttributes(true)
                                 where attrs.Any(attr => attr is PropertyAttribute)
                                 select property;

                // We now need to serialized all properties
                var serializedProperties = new List<SerializableProperty>();

                try
                {
                    foreach (var property in properties)
                    {
                        var serialized = property.ToSerializable();
                        serializedProperties.Add(serialized);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(PLUGIN_NAME, $"An Error occured while serializing a property from '{pluginName}': {ex.Message}", LogLevel.Error);
                    continue;
                }

                Plugins.Add(
                    new SerializablePlugin(pluginName,
                                           plugin.FullName,
                                           IdentifierPluginPair.Key,
                                           serializedProperties)
                    {
                        Type = PluginType.Binding
                    }
                );
            }

            Log.Write(PLUGIN_NAME, $"Found {Plugins.Count} Usable Bindings Plugins.");
        }

        #endregion
    }
}
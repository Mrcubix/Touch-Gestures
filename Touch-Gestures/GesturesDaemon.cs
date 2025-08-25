using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
using TouchGestures.Lib.Entities.Tablet;
using AppInfo = OpenTabletDriver.Desktop.AppInfo;

namespace TouchGestures
{
    /// <summary>
    ///   Manages settings for each tablets as well as the RPC server.
    /// </summary>
    [PluginName(PLUGIN_NAME)]
    public sealed class GesturesDaemon : GesturesDaemonBase, ITool
    {
        #region Fields

        private Vector2 _tabletSize = Vector2.One;

        private Vector2 _touchLPMM = Vector2.One;

        #endregion

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

            TabletAdded += OnTabletAdded;
        }

        public GesturesDaemon(Settings settings) : base()
        {
            TouchGestureSettings = settings;

            Initialize(false);
        }

        #endregion

        #region RPC Methods

        protected override void SerializePlugins()
        {
            Plugins.Clear();

            foreach (var IdentifierPluginPair in IdentifierToPluginConversion)
            {
                var plugin = IdentifierPluginPair.Value;

                var type = AppInfo.PluginManager.PluginTypes.FirstOrDefault(t => t.FullName == plugin.FullName);

                if (type == null)
                    continue; // type doesn't exist 

                var store = new PluginSettingStore(plugin);
                var binding = store.Construct<IBinding>();

                // There are situation where the name isn't specified, in which case we use the type's FullName
                var pluginName = plugin.GetCustomAttribute<PluginNameAttribute>()?.Name
                                 ?? plugin.FullName ?? $"Plugin {IdentifierPluginPair.Key}";

                // We only support properties decorated with the [Property] attribute OR a ValidProperties in IValidateBinding
                var properties = from property in type.GetProperties()
                                 let attrs = property.GetCustomAttributes(true)
                                 where attrs.Any(attr => attr is PropertyAttribute)
                                 select property;

                // We now need to serialized all properties
                var serializedProperties = new List<SerializableProperty>();

                if (binding is IValidateBinding validateBinding)
                {
                    serializedProperties.Add(new SerializableValidatedProperty(
                        "Property",
                        JTokenType.Array,
                        validateBinding.ValidProperties,
                        Array.Empty<SerializableAttributeModifier>()
                    ));
                }

                /*try
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
                }*/

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

        /// <inheritdoc />
        public override Task<Vector2> GetTabletSize()
        {
            Log.Write("Gestures Daemon", "Acquiring Tablet...");

            return Task.FromResult(_tabletSize);
        }

        /// <inheritdoc />
        public override Task<Vector2> GetTabletLinesPerMM()
        {
            Log.Write("Gestures Daemon", "Acquiring Tablet...");

            return Task.FromResult(_touchLPMM);
        }

        #endregion

        #region Event Handlers

        private void OnTabletAdded(object? sender, SharedTabletReference tablet)
        {
            _tabletSize = _tablets[0].Size;
            _touchLPMM = _tablets[0].TouchDigitizer?.GetLPMM() ?? Vector2.One;

            if (TouchGestureSettings == null)
                return;

            var profile = TouchGestureSettings.Profiles.Find(p => p.Name == _tablets[0].Name);

            if (profile == null)
                return;

            // Update gestures in profile with new LPMM
            foreach (var gesture in profile)
                gesture.LinesPerMM = _touchLPMM;
        }

        #endregion

        #region Disposal

        public override void Dispose()
        {
            TabletAdded -= OnTabletAdded;

            base.Dispose();
        }

        #endregion
    }
}
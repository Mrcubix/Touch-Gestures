using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using TouchGestures.Converters;
using TouchGestures.Entities;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Extensions;
using Settings = TouchGestures.Lib.Entities.Settings;

namespace TouchGestures
{
    /// <summary>
    ///   Manages settings for each tablets as well as the RPC server.
    /// </summary>
    [PluginName(PLUGIN_NAME)]
    public class GesturesDaemon : GesturesDaemonBase<BulletproofBindableProfile, BulletproofBindingSettingsStore>, ITool
    {
        #region Constructors

        public GesturesDaemon()
        {
#if DEBUG
            WaitForDebugger();
#endif
            if (_rpcServer == null)
            {
                _rpcServer = new RpcServer<GesturesDaemonBase>("GesturesDaemon", this);
                _rpcServer.Converters.Add(new SharedAreaConverter());
            }

            Settings.SerializerSettings.Converters.Add(new BulletproofBindingSettingStoreConverter());
            _pluginsTypes = AppInfo.PluginManager.GetChildTypes<IStateBinding>();

            Instance ??= this;
        }
        
        private void WaitForDebugger()
        {
            Console.WriteLine("Waiting for debugger to attach...");

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
        }

        #endregion

        #region RPC Methods

        public override Task<List<SerializablePlugin>> GetPlugins()
        {
            Log.Write("Gestures Daemon", "Getting plugins...");

            List<SerializablePlugin> plugins = new();

            foreach (var IdentifierPluginPair in IdentifierToPluginConversion)
            {
                var plugin = IdentifierPluginPair.Value;

                var store = new PluginSettingStore(plugin);

                // ALL that extra reflection bs just to get valid keys
                var property = store.GetTypeInfo()?.FindPropertyWithAttribute<PropertyValidatedAttribute>();

                if (property == null)
                    continue;

                var attribute = property.GetCustomAttribute<PropertyValidatedAttribute>();

                if (attribute == null)
                    continue;

                var validKeys = attribute.GetValue<IEnumerable<string>>(property);

                var serializablePlugin = new SerializablePlugin(plugin.GetCustomAttribute<PluginNameAttribute>()?.Name,
                                                                plugin.FullName,
                                                                IdentifierPluginPair.Key,
                                                                validKeys.ToArray());

                plugins.Add(serializablePlugin);
            }

            return Task.FromResult(plugins);
        }

        public override Task<bool> StartRecording()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> StopRecording()
        {
            return Task.FromResult(true);
        }

        #endregion
    }
}
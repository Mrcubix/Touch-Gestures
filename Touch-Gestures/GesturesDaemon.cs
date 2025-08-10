using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using TouchGestures.Lib;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;

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

            Instance ??= this;

            TabletAdded += OnTabletAdded;
        }

        public GesturesDaemon(Settings settings)
        {
            TouchGestureSettings = settings;

            Initialize(false);
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

        /// <inheritdoc />
        public override Task<List<SerializablePlugin>> GetPlugins()
        {
            Log.Write("Gestures Daemon", "Getting plugins...");

            List<SerializablePlugin> plugins = new();

            foreach (var IdentifierPluginPair in IdentifierToPluginConversion)
            {
                var plugin = IdentifierPluginPair.Value;

                var store = new PluginSettingStore(plugin);

                var validateBinding = store.Construct<IValidateBinding>();

                var serializablePlugin = new SerializablePlugin(plugin.GetCustomAttribute<PluginNameAttribute>()?.Name,
                                                                plugin.FullName,
                                                                IdentifierPluginPair.Key,
                                                                validateBinding.ValidProperties);

                plugins.Add(serializablePlugin);
            }

            Log.Write("Gestures Daemon", $"Found {plugins.Count} Usable Bindings Plugins.");

            return Task.FromResult(plugins);
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

        public override Task<bool> StartRecording()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> StopRecording()
        {
            return Task.FromResult(true);
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
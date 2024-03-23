using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;
using OTD.EnhancedOutputMode.Lib.Interface;
using OTD.EnhancedOutputMode.Tool;
using TouchGestures.Entities;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Entities.Gestures.Bases;

namespace TouchGestures
{
    [PluginName(PLUGIN_NAME)]
    public class GesturesHandler : IFilter, IGateFilter, IInitialize, IDisposable
    {
        #region Constants

        public const string PLUGIN_NAME = "Touch Gestures";

        private static readonly List<JsonConverter> Converters = new() { new SharedAreaConverter() };
        private readonly RpcServer<GesturesDaemon> rpcServer;

        #endregion

        #region Fields

        private Settings _settings = Settings.Default;
        private bool _hasPreviousGestureStarted;

        #endregion

        #region Constructors

        public GesturesHandler()
        {
#if DEBUG

            Console.WriteLine("Waiting for debugger to attach...");

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }

#endif

            // start the RPC server
            rpcServer = new RpcServer<GesturesDaemon>("GesturesDaemon");
            rpcServer.Converters.AddRange(Converters);

            rpcServer.Instance.OnSettingsChanged += OnSettingsChanged;
        }

        public void Initialize()
        {
            rpcServer.Instance.Initialize();

            _ = Task.Run(() => rpcServer.MainAsync());

            Log.Write(PLUGIN_NAME, "Initialized");
        }

        #endregion

        #region Properties

        public List<Gesture> ConflictingGestures { get; set; } = new();
        public List<Gesture> NonConflictingGestures { get; set; } = new();

        #endregion

        #region Methods

        public Vector2 Filter(Vector2 input) => input;

        public bool Pass(IDeviceReport report, ref ITabletReport tabletreport)
        {
            if (report is ITouchReport touchReport)
            {
                if (rpcServer.Instance.IsReady && TouchToggle.istouchToggled)
                {
                    _hasPreviousGestureStarted = false;

                    // Iterate through all conflicting gestures
                    foreach (var gesture in ConflictingGestures)
                    {
                        gesture.OnInput(touchReport.Touches);

                        // TODO: Ending it might not be the best move as simillar gesture might not work at all

                        // if the previous gesture has started, end any gesture after it
                        if (_hasPreviousGestureStarted)
                        {
                            gesture.End();
                            continue;
                        }

                        if (gesture.HasStarted)
                            _hasPreviousGestureStarted = true;
                    }

                    // Iterate through all non-conflicting gestures
                    foreach (var gesture in NonConflictingGestures)
                        gesture.OnInput(touchReport.Touches);
                }
            }

            return true;
        }

        #endregion

        #region Events Handlers

        public void OnSettingsChanged(object? sender, Settings? s)
        {
            if (s == null)
            {
                Log.Write(PLUGIN_NAME, "Settings are null", LogLevel.Error);
                return;
            }

            _settings = s;

            ConflictingGestures.Clear();
            NonConflictingGestures.Clear();

            _settings.TapGestures.Sort((x, y) => x.RequiredTouchesCount > y.RequiredTouchesCount ? -1 : 1);
            _settings.HoldGestures.Sort((x, y) => x.RequiredTouchesCount > y.RequiredTouchesCount ? -1 : 1);
            _settings.SwipeGestures.Sort((x, y) => x.Direction > y.Direction ? -1 : 1);
            _settings.PanGestures.Sort((x, y) => x.Direction > y.Direction ? -1 : 1);

            ConflictingGestures.AddRange(_settings.TapGestures);
            //ConflictingGestures.AddRange(_settings.HoldGestures);

            NonConflictingGestures.AddRange(_settings.SwipeGestures);
            NonConflictingGestures.AddRange(_settings.PanGestures);

            Log.Debug(PLUGIN_NAME, "Settings updated");
        }

        #endregion

        #region Interface Implementations

        public void Dispose()
        {
            // dispose of the settings
            _settings = null!;

            rpcServer.Instance.OnSettingsChanged -= OnSettingsChanged;

            // dispose of the rpc server
            rpcServer?.Dispose();
        }

        #endregion

        #region Parent Class Implementations

        public FilterStage FilterStage => FilterStage.PreTranspose;

        #endregion

        #region Plugin Properties

        #endregion
    }
}

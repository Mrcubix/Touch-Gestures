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
using OTD.EnhancedOutputMode.Lib.Tools;
using TouchGestures.Entities;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Entities.Gestures;
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

        public List<TapGesture> TapGestures { get; set; } = new();
        public List<HoldGesture> HoldGestures { get; set; } = new();

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
                    // Iterate through all conflicting gestures
                    HandleConflictingGestures(TapGestures, touchReport);
                    HandleConflictingGestures(HoldGestures, touchReport);

                    // Iterate through all non-conflicting gestures
                    foreach (var gesture in NonConflictingGestures)
                        gesture.OnInput(touchReport.Touches);
                }
            }

            return true;
        }

        public void HandleConflictingGestures(IEnumerable<Gesture> gestures, ITouchReport touchReport)
        {
            _hasPreviousGestureStarted = false;

            foreach (var gesture in gestures)
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

            TapGestures.Clear();
            HoldGestures.Clear();
            NonConflictingGestures.Clear();

            _settings.TapGestures.Sort((x, y) => x.RequiredTouchesCount > y.RequiredTouchesCount ? -1 : 1);
            _settings.HoldGestures.Sort((x, y) => x.RequiredTouchesCount > y.RequiredTouchesCount ? -1 : 1);
            _settings.SwipeGestures.Sort((x, y) => x.Direction > y.Direction ? -1 : 1);
            _settings.PanGestures.Sort((x, y) => x.Direction > y.Direction ? -1 : 1);
            _settings.PinchGestures.Sort((x, y) => x.IsInner ? -1 : 1);
            _settings.RotateGestures.Sort((x, y) => x.IsClockwise ? -1 : 1);

            TapGestures.AddRange(_settings.TapGestures);
            HoldGestures.AddRange(_settings.HoldGestures);

            NonConflictingGestures.AddRange(_settings.SwipeGestures);
            NonConflictingGestures.AddRange(_settings.PanGestures);
            NonConflictingGestures.AddRange(_settings.PinchGestures);
            NonConflictingGestures.AddRange(_settings.RotateGestures);

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

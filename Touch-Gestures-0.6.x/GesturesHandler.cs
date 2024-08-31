using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using OpenTabletDriver;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;
using OTD.EnhancedOutputMode.Lib.Tools;
using TouchGestures.Entities;
using TouchGestures.Extensions;
using TouchGestures.Lib;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures
{
    [PluginName(PLUGIN_NAME)]
    public class GesturesHandler : IPositionedPipelineElement<IDeviceReport>, IDisposable
    {
        #region Constants

        public const string PLUGIN_NAME = "Touch Gestures";

        #endregion

        #region Fields

        private GesturesDaemonBase? _daemon;
        private TouchSettings _touchSettings = TouchSettings.Default;
        private IOutputMode? _outputMode;
        private BindableProfile? _profile;
        private SharedTabletReference? _tablet;
        private bool _awaitingDaemon;
        private bool _hasPreviousGestureStarted;


        #endregion

        #region Initialization

        public GesturesHandler()
        {
#if DEBUG
            WaitForDebugger();
            Log.Write(PLUGIN_NAME, "Debugger attached", LogLevel.Debug);
#endif

            GesturesDaemonBase.DaemonLoaded += OnDaemonLoaded;
            _awaitingDaemon = true;

            BindingBuilder.Current = new BulletproofBindingBuilder();
            Logger.Instance = new BulletproofLogger();
        }

        private static void WaitForDebugger()
        {
            Console.WriteLine("Waiting for debugger to attach...");

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
        }

        public void Initialize()
        {
            FetchTouchSettings();

            if (!_touchSettings.IsTouchToggled)
                return;

            // Filters are loaded before tools for some reasons, so we have to wait for the daemon to be loaded
            _daemon = GesturesDaemonBase.Instance;

            // OTD 0.6.4.0 doesn't dispose of plugins when detecting tablets, so unsubscribing early is necessary
            GesturesDaemonBase.DaemonLoaded -= OnDaemonLoaded;
            _awaitingDaemon = false;

            if (Tablet != null)
                InitializeCore(Tablet);

            if (_daemon == null)
                Log.Write(PLUGIN_NAME, "Touch Gestures Daemon has not been enabled, please enable it in the 'Tools' tab", LogLevel.Error);
        }

        private void InitializeCore(TabletReference tablet)
        {
            _tablet = tablet.ToShared(_touchSettings);

            AddServices();

            if (_daemon != null)
            {
                _daemon.AddTablet(_tablet);
                _profile = _daemon.GetSettingsForTablet(_tablet.Name);

                if (_profile != null)
                {
                    _profile.ProfileChanged += OnProfileChanged;
                    OnProfileChanged(this, EventArgs.Empty);
                }

                Log.Write(PLUGIN_NAME, "Now handling touch gesture for: " + _tablet.Name);
            }
        }

        private void FetchTouchSettings()
        {
            if (_Driver is Driver driver && Tablet != null)
            {
                // fetch the device first
                var device = driver.InputDevices.Where(x => x.Properties.Name == Tablet.Properties.Name).FirstOrDefault();

                // then fetch the output mode
                _outputMode = device?.OutputMode;

                // then fetch the touch settings
                var settings = _outputMode?.Elements.OfType<TouchSettings>().FirstOrDefault();

                if (settings != null)
                    _touchSettings = settings;
                else
                    Log.Write(PLUGIN_NAME, "Touch settings are null, using default values", LogLevel.Warning);
            }
        }

        private void AddServices()
        {
            if (_tablet is BulletproofSharedTabletReference btablet)
            {
                object? pointer = _outputMode switch
                {
                    AbsoluteOutputMode absoluteOutputMode => absoluteOutputMode.Pointer,
                    RelativeOutputMode relativeOutputMode => relativeOutputMode.Pointer,
                    _ => null
                };

                if (pointer is IMouseButtonHandler mouseButtonHandler)
                    btablet.ServiceProvider.AddService(() => mouseButtonHandler);
            }
        }

        #endregion

        #region Properties

        public List<TapGesture> TapGestures { get; set; } = new();
        public List<HoldGesture> HoldGestures { get; set; } = new();

        public List<Gesture> NonConflictingGestures { get; set; } = new();

        [TabletReference]
        public TabletReference? Tablet { get; set; }

        [Resolved]
        public IDriver? _Driver { set; get; }

        public PipelinePosition Position => PipelinePosition.PreTransform;

        #endregion

        #region Events

        public event Action<IDeviceReport>? Emit;

        #endregion

        #region Methods

        public void Consume(IDeviceReport report)
        {
            if (report is ITouchReport touchReport)
            {
                if (_daemon != null && _daemon.IsReady && _touchSettings.IsTouchToggled)
                {
                    // Iterate through all conflicting gestures
                    HandleConflictingGestures(TapGestures, touchReport);
                    HandleConflictingGestures(HoldGestures, touchReport);

                    // Iterate through all non-conflicting gestures
                    foreach (var gesture in NonConflictingGestures)
                        gesture.OnInput(touchReport.Touches.FromOTD());
                }
            }

            Emit?.Invoke(report);
        }

        public void HandleConflictingGestures(IEnumerable<Gesture> gestures, ITouchReport touchReport)
        {
            _hasPreviousGestureStarted = false;

            foreach (var gesture in gestures)
            {
                gesture.OnInput(touchReport.Touches.FromOTD());

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

        public void OnDaemonLoaded(object? sender, EventArgs e)
            => Initialize();

        public void OnProfileChanged(object? sender, EventArgs e)
        {
            if (_profile == null)
            {
                Log.Write(PLUGIN_NAME, "Settings are null", LogLevel.Error);
                return;
            }

            if (_tablet != null)
            {
                if (_tablet.TouchDigitizer != null && _tablet.TouchDigitizer.GetLPMM() != Vector2.Zero)
                    _profile.UpdateLPMM(_tablet);
                else
                    Log.Write(PLUGIN_NAME, "LPMM is zero, this usually means that 'Touch Settings' hasn't been enabled or its maxes are set to zero", LogLevel.Error);
            }

            TapGestures.Clear();
            HoldGestures.Clear();
            NonConflictingGestures.Clear();

            SortGestures();

            TapGestures.AddRange(_profile.TapGestures);
            HoldGestures.AddRange(_profile.HoldGestures);

            NonConflictingGestures.AddRange(_profile.SwipeGestures);
            NonConflictingGestures.AddRange(_profile.PanGestures);
            NonConflictingGestures.AddRange(_profile.PinchGestures);
            NonConflictingGestures.AddRange(_profile.RotateGestures);

            Log.Debug(PLUGIN_NAME, "Settings updated");
        }

        private void SortGestures()
        {
            if (_profile == null)
                return;

            _profile.TapGestures.Sort((x, y) => x.RequiredTouchesCount > y.RequiredTouchesCount ? -1 : 1);
            _profile.HoldGestures.Sort((x, y) => x.RequiredTouchesCount > y.RequiredTouchesCount ? -1 : 1);
            _profile.SwipeGestures.Sort((x, y) => x.Direction > y.Direction ? -1 : 1);
            _profile.PanGestures.Sort((x, y) => x.Direction > y.Direction ? -1 : 1);
            _profile.PinchGestures.Sort((x, y) => x.IsInner ? -1 : 1);
            _profile.RotateGestures.Sort((x, y) => x.IsClockwise ? -1 : 1);
        }

        #endregion

        #region Interface Implementations

        public void Dispose()
        {
            // Unsubscribe from events
            if (_profile != null)
                _profile.ProfileChanged -= OnProfileChanged;

            if (_awaitingDaemon)
                GesturesDaemonBase.DaemonLoaded -= OnDaemonLoaded;

            _awaitingDaemon = false;

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

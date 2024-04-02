using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime;
using System.Threading;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;
using OTD.EnhancedOutputMode.Lib.Interface;
using OTD.EnhancedOutputMode.Lib.Tools;
using TouchGestures.Extensions;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures
{
    [PluginName(PLUGIN_NAME)]
    public class GesturesHandler : IFilter, IGateFilter, IDisposable
    {
        #region Constants

        public const string PLUGIN_NAME = "Touch Gestures";

        #endregion

        #region Fields

        private GesturesDaemonBase? _daemon;
        private BindableProfile? _profile;
        private SharedTabletReference? _tablet;
        private bool _hasPreviousGestureStarted;

        #endregion

        #region Constructors

        public GesturesHandler()
        {
#if DEBUG
            WaitForDebugger();
#endif

            GesturesDaemonBase.DaemonLoaded += OnDaemonLoaded;
        }

        public void Initialize()
        {
            _daemon = GesturesDaemonBase.Instance;

            if (Info.Driver.Tablet != null && _daemon != null)
            {
                _tablet = Info.Driver.Tablet.ToShared();
                _daemon.AddTablet(_tablet);
                _profile = _daemon.GetSettingsForTablet(_tablet.Name);

                if (_profile != null)
                {
                    _profile.ProfileChanged += OnProfileChanged;
                    OnProfileChanged(this, EventArgs.Empty);
                }

                Log.Write(PLUGIN_NAME, "Now handling touch gesture for: " + _tablet.Name);
            }

            if (_daemon == null)
            {
                Log.Write(PLUGIN_NAME, "Touch Gestures Daemon has not been enabled, please enable it in the 'Tools' tab", LogLevel.Error);
                return;
            }
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

        #region Properties

        public List<TapGesture> TapGestures { get; set; } = new();
        public List<HoldGesture> HoldGestures { get; set; } = new();
        public List<Gesture> NonConflictingGestures { get; set; } = new();

        public FilterStage FilterStage => FilterStage.PreTranspose;

        #endregion

        #region Methods

        public Vector2 Filter(Vector2 input) => input;

        public bool Pass(IDeviceReport report, ref ITabletReport tabletreport)
        {
            if (report is ITouchReport touchReport)
            {
                if (_daemon != null && _daemon.IsReady && TouchSettings.istouchToggled)
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

        public void OnDaemonLoaded(object? sender, EventArgs e)
        {
            Initialize();
        }

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

            GesturesDaemonBase.DaemonLoaded -= OnDaemonLoaded;

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

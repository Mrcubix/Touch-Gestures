using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Lib
{
    public abstract class GesturesHandlerBase : IDisposable
    {
        #region Constants

        protected const string PLUGIN_NAME = "Touch Gestures";

        #endregion

        #region Fields

        protected GesturesDaemonBase? _daemon;
        protected BindableProfile? _profile;
        protected SharedTabletReference? _tablet;
        private bool _hasPreviousGestureStarted;

        #endregion

        #region Constructors

        protected GesturesHandlerBase()
        {
#if DEBUG
            WaitForDebugger();
            Log.Write(PLUGIN_NAME, "Debugger attached", LogLevel.Debug);
#endif
        }

        public abstract void Initialize();

        #endregion

        #region Properties

        public List<TapGesture> TapGestures { get; set; } = new();
        public List<HoldGesture> HoldGestures { get; set; } = new();
        public List<Gesture> NonConflictingGestures { get; set; } = new();

        #endregion

        #region Methods

        public virtual bool Consume(IDeviceReport report, bool IsTouchToggled = true)
        {
            if (report is ITouchReport touchReport)
            {
                if (_daemon != null && _daemon.IsReady && IsTouchToggled)
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

        protected void HandleConflictingGestures(IEnumerable<Gesture> gestures, ITouchReport touchReport)
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

        public virtual void OnProfileChanged(object? sender, EventArgs e)
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
            
            ReplaceGesturesUsingProfile();

            Log.Debug(PLUGIN_NAME, "Settings updated");
        }

        protected void SortGestures()
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

        protected void ReplaceGesturesUsingProfile()
        {
            if (_profile == null)
            {
                Log.Write(PLUGIN_NAME, "Settings are null", LogLevel.Error);
                return;
            }

            TapGestures.Clear();
            HoldGestures.Clear();
            NonConflictingGestures.Clear();

            SortGestures();

            // TODO : Non-Conflicting Tap & Hold gestures should be added to NonConflictingGestures
            TapGestures.AddRange(_profile.TapGestures);
            HoldGestures.AddRange(_profile.HoldGestures);

            NonConflictingGestures.AddRange(_profile.SwipeGestures);
            NonConflictingGestures.AddRange(_profile.PanGestures);
            NonConflictingGestures.AddRange(_profile.PinchGestures);
            NonConflictingGestures.AddRange(_profile.RotateGestures);
        }

        #endregion

        #region Interface Implementations

        public virtual void Dispose()
        {
            // Unsubscribe from events
            if (_profile != null)
                _profile.ProfileChanged -= OnProfileChanged;

            if (_tablet != null)
                _daemon?.RemoveTablet(_tablet);

            GC.SuppressFinalize(this);
        }

        #endregion

        private static void WaitForDebugger()
        {
            Console.WriteLine("Waiting for debugger to attach...");

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
        }
    }
}

using System;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Extensions;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures
{
    [PluginName(PLUGIN_NAME)]
    public class PenGesturesHandler : GesturesHandler
    {
        #region Constants

        private const string PLUGIN_NAME = "Pen Gestures";

        #endregion

        #region Fields

        private readonly TouchReport _stubReport = new(1);
        private readonly TouchPoint _stubPoint = new();

        #endregion

        #region Initialization

        protected override void InitializeCore(TabletReference tablet)
        {
            _tablet = tablet.ToShared(_touchSettings);
            _tablet.Name = $"{_tablet.Name} (Pen Only)";

            AddServices();

            if (_daemon != null)
            {
                _daemon.AddTablet(_tablet);
                _profile = _daemon.GetSettingsForTablet(_tablet.Name);

                if (_profile != null)
                {
                    _profile.IsMultiTouch = false;
                    _profile.ProfileChanged += OnProfileChanged;
                    OnProfileChanged(this, EventArgs.Empty);
                }

                Log.Write(PLUGIN_NAME, "Now handling touch gesture for: " + _tablet.Name);
            }
        }

        #endregion

        #region Methods

        public override void Consume(IDeviceReport report)
        {
            if (report is ITabletReport tabletReport)
            {
                if (tabletReport.Pressure > 0)
                {
                    _stubPoint.Position = tabletReport.Position;
                    _stubReport.Touches[0] = _stubPoint;
                }
                else
                    _stubReport.Touches[0] = null;
                
                if (_daemon != null && _daemon.IsReady)
                {
                    // Iterate through all conflicting gestures
                    HandleConflictingGestures(TapGestures, _stubReport);
                    HandleConflictingGestures(HoldGestures, _stubReport);

                    // Iterate through all non-conflicting gestures
                    foreach (var gesture in NonConflictingGestures)
                        gesture.OnInput(_stubReport.Touches);
                }
            }

            OnEmit(report);
        }

        #endregion

        #region Events Handlers

        public override void OnProfileChanged(object? sender, EventArgs e)
        {
            if (_profile == null)
            {
                Log.Write(PLUGIN_NAME, "Settings are null", LogLevel.Error);
                return;
            }

            if (_tablet != null)
            {
                var lpmm = _tablet.PenDigitizer?.GetLPMM() ?? Vector2.Zero;

                if (_tablet.PenDigitizer != null && lpmm != Vector2.Zero)
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

        #endregion
    }
}

using System;
using System.Linq;
using System.Numerics;
using OpenTabletDriver;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Extensions;
using TouchGestures.Lib;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures
{
    [PluginName(PLUGIN_NAME)]
    public class PenGesturesHandler : GesturesHandler
    {
        #region Constants

        private new const string PLUGIN_NAME = "Pen Gestures";

        #endregion

        #region Fields

        private readonly TouchReport _stubReport = new(1);
        private readonly TouchPoint _stubPoint = new();

        #endregion

        #region Initialization

        public override void Initialize()
        {
            FetchOutputMode();

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
            Consume(report, true);
            OnEmit(report);
        }

        public override bool Consume(IDeviceReport report, bool IsTouchToggled = true)
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

            return true;
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

            ReplaceGesturesUsingProfile();

            Log.Debug(PLUGIN_NAME, "Settings updated");
        }

        #endregion
    }
}

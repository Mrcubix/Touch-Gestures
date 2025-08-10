using System;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Extensions;
using TouchGestures.Lib;
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

        #region Constructors

        public override void Initialize()
        {
            _daemon = GesturesDaemonBase.Instance;

            if (Info.Driver.Tablet != null && _daemon != null)
            {
                _tablet = Info.Driver.Tablet.ToShared(_touchSettings);
                _tablet.Name = $"{_tablet.Name} (Pen Only)"; 

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

            if (_daemon == null)
            {
                Log.Write(PLUGIN_NAME, "Touch Gestures Daemon has not been enabled, please enable it in the 'Tools' tab", LogLevel.Error);
                return;
            }
        }

        #endregion

        #region Methods

        public override bool Pass(IDeviceReport report, ref ITabletReport tabletreport)
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

                if (_daemon != null && _daemon.IsReady && _touchSettings.IsTouchToggled)
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
                if (_tablet.TouchDigitizer != null && _tablet.PenDigitizer?.GetLPMM() != Vector2.Zero)
                    _profile.UpdateLPMM(_tablet);
                else
                    Log.Write(PLUGIN_NAME, "LPMM is zero, this is very unusual as the tablet's specifications should be defined in the internal tablet configuration.", LogLevel.Error);
            }

            ReplaceGesturesUsingProfile();

            Log.Debug(PLUGIN_NAME, "Settings updated");
        }

        #endregion
    }
}

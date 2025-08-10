using System;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OTD.EnhancedOutputMode.Lib.Interface;
using OTD.EnhancedOutputMode.Lib.Tools;
using TouchGestures.Extensions;
using TouchGestures.Lib;

namespace TouchGestures
{
    [PluginName(PLUGIN_NAME)]
    public class GesturesHandler : GesturesHandlerBase, IFilter, IGateFilter, IInitialize, IDisposable
    {
        #region Fields

        protected TouchSettings _touchSettings => TouchSettings.Instance ?? TouchSettings.Default;

        #endregion

        #region Initialization

        public GesturesHandler() : base() { }

        public override void Initialize()
        {
            if (!_touchSettings.IsTouchToggled)
                return;

            _daemon = GesturesDaemonBase.Instance;

            if (Info.Driver.Tablet != null && _daemon != null)
            {
                _tablet = Info.Driver.Tablet.ToShared(_touchSettings);
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

        #endregion

        #region Properties

        public FilterStage FilterStage => FilterStage.PreTranspose;

        #endregion

        #region Methods

        public Vector2 Filter(Vector2 input) => input;

        public virtual bool Pass(IDeviceReport report, ref ITabletReport tabletreport)
            => Consume(report, _touchSettings.IsTouchToggled);

        #endregion
    }
}

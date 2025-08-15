using System;
using System.Linq;
using OpenTabletDriver;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using OTD.EnhancedOutputMode.Lib.Tools;
using TouchGestures.Entities;
using TouchGestures.Extensions;
using TouchGestures.Lib;

namespace TouchGestures
{
    [PluginName(PLUGIN_NAME)]
    public class GesturesHandler : GesturesHandlerBase, IPositionedPipelineElement<IDeviceReport>, IDisposable
    {
        #region Fields

        protected TouchSettings _touchSettings = TouchSettings.Default;
        protected IOutputMode? _outputMode;
        protected bool _awaitingDaemon;

        #endregion

        #region Initialization

        public GesturesHandler() : base()
        {
            GesturesDaemonBase.DaemonLoaded += OnDaemonLoaded;
            _awaitingDaemon = true;

            BulletproofBindingBuilder.ChooseAsBuilder();
        }

        public override void Initialize()
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

                _daemon.RecordingRequested += OnRecordingRequested;
                _daemon.RecordingStopped += OnRecordingStopped;

                Log.Write(PLUGIN_NAME, "Now handling touch gesture for: " + _tablet.Name);
            }
        }

        protected void FetchTouchSettings()
        {
            if (_outputMode == null)
                FetchOutputMode();

            // then fetch the touch settings
            var settings = _outputMode?.Elements.OfType<TouchSettings>().FirstOrDefault();

            if (settings != null)
                _touchSettings = settings;
            else
                Log.Write(PLUGIN_NAME, "Touch settings are null, using default values", LogLevel.Warning);
        }

        protected void FetchOutputMode()
        {
            if (_Driver is Driver driver && Tablet != null)
            {
                // fetch the device first
                var device = driver.InputDevices.Where(x => x.Properties.Name == Tablet.Properties.Name).FirstOrDefault();

                // then fetch the output mode
                _outputMode = device?.OutputMode;
            }
        }

        protected void AddServices()
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

        public virtual void Consume(IDeviceReport report)
        {
            Consume(report, _touchSettings.IsTouchToggled);
            Emit?.Invoke(report);
        }

        #endregion

        #region Events Handlers

        public void OnEmit(IDeviceReport e)
            => Emit?.Invoke(e);

        public void OnDaemonLoaded(object? sender, EventArgs e)
            => Initialize();

        #endregion

        #region Interface Implementations

        public override void Dispose()
        {
            base.Dispose();

            if (_awaitingDaemon)
                GesturesDaemonBase.DaemonLoaded -= OnDaemonLoaded;

            _awaitingDaemon = false;

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

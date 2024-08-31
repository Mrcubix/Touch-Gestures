using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Lib.Contracts;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Reflection;

namespace TouchGestures.Lib.Entities
{
    public class GesturesDaemonBase : IGesturesDaemon, IDisposable
    {
        #region Location 

        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private static readonly FileInfo _assemblyFile = new(_assembly.Location);

        // Plugin -> Containing Folder -> Plugins Folder -> Settings Folder
        private static readonly DirectoryInfo? _settingsFolder = _assemblyFile.Directory.Parent.Parent;

        #endregion

        protected static RpcServer<GesturesDaemonBase> _rpcServer = null!;
        public static GesturesDaemonBase Instance { get; protected set; } = null!;
        public static event EventHandler? DaemonLoaded;

        #region Constants

        public const string PLUGIN_NAME = "Touch Gestures Daemon";

        protected readonly string _settingsPath = Path.Combine(_settingsFolder!.FullName, "Touch-Gestures.json");

        protected readonly JsonSerializerSettings _serializerSettings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new SharedAreaConverter()
            }
        };

        #endregion

        #region Fields

        protected Logger? _logger = Logger.Instance;
        protected IReadOnlyCollection<TypeInfo> _pluginsTypes = new List<TypeInfo>();
        protected List<SharedTabletReference> _tablets = new();

        #endregion

        #region Initialization

        public virtual bool Initialize()
        {
            Initialize(true);

            if (HasErrored)
            {
                _logger?.Write("Gestures Daemon", "Failed to Initialize, aborting early.");
                return false;
            }

            _ = Task.Run(() => _rpcServer.MainAsync());

            _logger?.Write(PLUGIN_NAME, "Initialized");

            return true;
        }

        protected virtual void Initialize(bool doLoadSettings = true) {}

        protected virtual void IdentifyPlugins()
        {
            IdentifierToPluginConversion.Clear();

            foreach (var plugin in _pluginsTypes)
            {
                var identifierHash = 0;

                if (plugin.FullName == null)
                    continue;

                for (var i = 0; i < plugin.FullName.Length; i++)
                    identifierHash += plugin.FullName[i];

                if (!IdentifierToPluginConversion.ContainsKey(identifierHash))
                    IdentifierToPluginConversion.Add(identifierHash, plugin);
            }
        }

        #endregion

        #region Events

        public event EventHandler? Ready;
        public event EventHandler<Settings?>? SettingsChanged;
        public event EventHandler<IEnumerable<SharedTabletReference>>? TabletsChanged;

        protected event EventHandler<SharedTabletReference>? TabletAdded;
        protected event EventHandler<SharedTabletReference>? TabletRemoved;

        #endregion

        #region Properties

        public Dictionary<int, TypeInfo> IdentifierToPluginConversion = new();

        public Settings? TouchGestureSettings { get; protected set; } = Settings.Default;

        public bool IsReady { get; protected set; }

        public bool HasErrored { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Gets the settings for a specific tablet.
        /// </summary>
        /// <param name="name">The name of the tablet.</param>
        /// <returns>The settings for the tablet.</returns>
        public virtual BindableProfile GetSettingsForTablet(string name)
            => throw new NotImplementedException();

        public virtual bool AddTablet(SharedTabletReference tablet)
        {
            if (_tablets.Any(t => t.Name == tablet.Name))
                return false;

            _tablets.Add(tablet);

            return true;
        }

        public void RemoveTablet(SharedTabletReference tablet)
        {
            if (!_tablets.Remove(tablet))
                return;

            TabletsChanged?.Invoke(this, _tablets);
            TabletRemoved?.Invoke(this, tablet);
        }

        public void RemoveTablet(string name)
        {
            var tablet = _tablets.Find(t => t.Name == name);

            if (tablet != null)
                RemoveTablet(tablet);
        }

        #endregion

        #region Event Methods

        protected virtual void OnReady(EventArgs e)
            => Ready?.Invoke(this, e);

        protected virtual void OnSettingsChanged(Settings settings)
            => SettingsChanged?.Invoke(this, settings);

        protected virtual void OnTabletsChanged(IEnumerable<SharedTabletReference> tablets)
            => TabletsChanged?.Invoke(this, tablets);

        protected virtual void OnTabletAdded(SharedTabletReference tablet)
            => TabletAdded?.Invoke(this, tablet);

        protected virtual void OnTabletRemoved(SharedTabletReference tablet)
            => TabletRemoved?.Invoke(this, tablet);

        #endregion

        #region RPC Methods

        /// <summary>
        ///   Gets the plugins that are available for use.
        /// </summary>
        /// <remarks>
        ///   The method of obtaining plugins depends on the version of OpenTabletDriver.
        /// </remarks>
        /// <returns>The available plugins.</returns>
        public virtual Task<List<SerializablePlugin>> GetPlugins() 
            => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual Task<bool> IsTabletConnected()
        {
            _logger?.Write("Gestures Daemon", "Checking if tablet is connected...");

            return Task.FromResult(_tablets.Any());
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<SharedTabletReference>> GetTablets()
        {
            if (!_tablets.Any())
                return Task.FromResult(Array.Empty<SharedTabletReference>().AsEnumerable());

            _logger?.Write("Gestures Daemon", "Getting tablets...");

            return Task.FromResult(_tablets.AsEnumerable());
        }

        /// <inheritdoc />
        public virtual Task<Vector2> GetTabletSize()
            => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual Task<Vector2> GetTabletLinesPerMM()
            => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual Task<SerializableSettings> GetSettings()
            => throw new NotImplementedException("GetSettings is not implemented on the base class.");

        /// <inheritdoc />
        public virtual Task<bool> SaveSettings()
            => throw new NotImplementedException("SaveSettings is not implemented on the base class.");

        /// <inheritdoc />
        public virtual Task<bool> UpdateProfile(SerializableProfile profile)
            => throw new NotImplementedException("UpdateProfile is not implemented on the base class.");

        public virtual Task<bool> UpdateSettings(SerializableSettings settings)
            => throw new NotImplementedException("UpdateSettings is not implemented on the base class.");
        
        /// <inheritdoc />
        public virtual Task<bool> StartRecording()
            => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual Task<bool> StopRecording()
            => throw new NotImplementedException();

        #endregion

        #region Disposal

        public virtual void Dispose()
        {
            _tablets.Clear();
            _rpcServer.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Event Handlers

        protected void OnSettingsChanged(object sender, Settings? settings)
        {
            SettingsChanged?.Invoke(sender, settings);
        }

        protected void OnReady(object sender, EventArgs e)
        {
            Ready?.Invoke(sender, e);
        }

        protected static void OnDaemonLoaded(object? sender = null)
        {
            DaemonLoaded?.Invoke(sender, EventArgs.Empty);
        }

        #endregion
    }

    public class GesturesDaemonBase<TProfile, TStore> : GesturesDaemonBase
        where TProfile : BindableProfile, new()
        where TStore : BindingSettingStore, new()
    {
        public Settings<TProfile, TStore>? CurrentSettings => TouchGestureSettings as Settings<TProfile, TStore>;

        #region Initialization

        protected override void Initialize(bool doLoadSettings = true)
        {
            if (doLoadSettings)
                LoadSettings(_settingsPath);

            if (HasErrored)
            {
                _logger?.Write("Gestures Daemon", "Failed to load settings, aborting initialization.");
                return;
            }

            // identify plugins
            IdentifyPlugins();

            // construct bindings
            //TouchGestureSettings?.ConstructBindings();

            OnSettingsChanged(this, TouchGestureSettings);

            IsReady = true;
            OnReady(this, EventArgs.Empty);
            OnDaemonLoaded(this);
        }

        protected virtual void LoadSettings(string path)
        {
            if (!File.Exists(path))
                Settings.TrySaveTo(path, TouchGestureSettings!);

            HasErrored = !Settings.TryLoadFrom(path, out Settings<TProfile, TStore>? temp);

            if (!HasErrored)
                TouchGestureSettings = temp;
        }

        #endregion

        #region Methods

        public override bool AddTablet(SharedTabletReference tablet)
        {
            if (!base.AddTablet(tablet))
                return false;

            // Since a new tablet has been added, we need to build the bindings for it
            BuildProfileBindings(tablet);

            OnTabletsChanged(_tablets);
            OnTabletAdded(tablet);

            return true;
        }

        public override BindableProfile GetSettingsForTablet(string name)
        {
            var tablet = _tablets.Find(t => t.Name == name);

            if (tablet == null)
                return null!;

            return CurrentSettings?.Profiles.Find(p => p.Name == tablet.Name) ?? CreateProfileForTablet(tablet);
        }

        protected virtual BindableProfile CreateProfileForTablet(SharedTabletReference tablet)
        {
            if (CurrentSettings == null)
                return null!;

            var profile = new TProfile
            {
                Name = tablet.Name
            };

            CurrentSettings.Profiles.Add(profile);

            return profile;
        }

        protected virtual void BuildProfileBindings(SharedTabletReference tablet)
        {
            if (CurrentSettings == null)
                return;

            var profile = CurrentSettings?.Profiles.Find(p => p.Name == tablet.Name);

            if (profile == null)
                return;

            profile.ConstructBindings(tablet);
        }

        #endregion

        #region RPC Methods

        /// <inheritdoc />
        public override Task<SerializableSettings> GetSettings()
        {
            _logger?.Write("Gestures Daemon", "Converting Settings into a serializable form...");

            if (TouchGestureSettings == null)
                return Task.FromResult<SerializableSettings>(null!);

            var serializedSettings = TouchGestureSettings.ToSerializable(IdentifierToPluginConversion);

            return Task.FromResult(serializedSettings);
        }

        public override Task<bool> SaveSettings()
        {
            if (TouchGestureSettings == null)
                return Task.FromResult(false);

            return Task.Run(() => TouchGestureSettings.TrySaveTo(_settingsPath));
        }

        /// <inheritdoc />
        public override Task<bool> UpdateSettings(SerializableSettings settings) 
        {
            _logger?.Write("Gestures Daemon", "Updating settings...");

            if (settings == null)
                return Task.FromResult(false);

            TouchGestureSettings = new Settings<TProfile, TStore>(settings, IdentifierToPluginConversion);
            OnSettingsChanged(this, TouchGestureSettings);

            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public override Task<bool> UpdateProfile(SerializableProfile profile) 
        {
            _logger?.Write("Gestures Daemon", "Updating profile...");

            if (profile == null)
                return Task.FromResult(false);

            if (TouchGestureSettings is not Settings<TProfile, TStore> settings)
                return Task.FromResult(false);

            var bindableProfile = settings?.Profiles.Find(p => p.Name == profile.Name);
            var tablet = _tablets.Find(t => t.Name == profile.Name);

            if (bindableProfile == null || tablet == null)
                return Task.FromResult(false);

            bindableProfile.Update<TProfile, TStore>(profile, tablet, IdentifierToPluginConversion);

            return Task.FromResult(true);
        }

        #endregion
    }
}
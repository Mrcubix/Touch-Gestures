using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using TouchGestures.Converters;
using TouchGestures.Entities;
using TouchGestures.Extensions;
using TouchGestures.Lib.Contracts;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures
{
    /// <summary>
    ///   Manages settings for each tablets as well as the RPC server.
    /// </summary>
    [PluginName(PLUGIN_NAME)]
    public sealed class GesturesDaemon : ITool, IGesturesDaemon
    {
        private static RpcServer<GesturesDaemon> _rpcServer = null!;
        public static GesturesDaemon Instance { get; private set; } = null!;

        #region Constants

        public const string PLUGIN_NAME = "Touch Gestures Daemon";

        private readonly string _settingsPath = Path.Combine(OpenTabletDriver.Desktop.AppInfo.Current.AppDataDirectory, "Touch-Gestures.json");

        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new PluginSettingStoreConverter(),
                new PluginSettingConverter(),
                new SharedAreaConverter()
            }
        };

        #endregion

        #region Fields

        private List<SharedTabletReference> _tablets = new();

        private Vector2 TabletSize = Vector2.One;

        private Vector2 TouchLPMM = Vector2.One;

        #endregion

        #region Constructors

        public GesturesDaemon()
        {
#if DEBUG
            WaitForDebugger();
#endif
            if (_rpcServer == null)
            {
                _rpcServer = new RpcServer<GesturesDaemon>("GesturesDaemon", this);
                _rpcServer.Converters.Add(new SharedAreaConverter());
            }

            Instance ??= this;

            TabletAdded += OnTabletsAdded;
        }

        private void WaitForDebugger()
        {
            Console.WriteLine("Waiting for debugger to attach...");

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
        }

        public GesturesDaemon(Settings settings)
        {
            TouchGestureSettings = settings;

            Initialize(false);
        }

        public bool Initialize()
        {
            Initialize(true);

            if (HasErrored)
            {
                Log.Write("Gestures Daemon", "Failed to Initialize, aborting early.");
                return false;
            }

            _ = Task.Run(() => _rpcServer.MainAsync());

            Log.Write(PLUGIN_NAME, "Initialized");

            return true;
        }

        private void Initialize(bool doLoadSettings = true)
        {
            if (doLoadSettings)
                LoadSettings(_settingsPath);

            if (HasErrored)
            {
                Log.Write("Gestures Daemon", "Failed to load settings, aborting initialization.");
                return;
            }

            // identify plugins
            IdentifyPlugins();

            // construct bindings
            TouchGestureSettings?.ConstructBindings();

            OnSettingsChanged?.Invoke(this, TouchGestureSettings);

            IsReady = true;
            OnReady?.Invoke(this, null!);
        }

        #endregion

        #region Events

        public event EventHandler? OnReady;
        public event EventHandler<Settings?>? OnSettingsChanged;
        public event EventHandler<IEnumerable<SharedTabletReference>>? TabletsChanged;

        private event EventHandler<SharedTabletReference>? TabletAdded;
        private event EventHandler<SharedTabletReference>? TabletRemoved;

        #endregion

        #region Properties

        public Dictionary<int, TypeInfo> IdentifierToPluginConversion = new();

        public Settings? TouchGestureSettings { get; private set; } = Settings.Default;

        public bool IsReady { get; private set; }

        public bool HasErrored { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Gets the settings for a specific tablet.
        /// </summary>
        /// <param name="name">The name of the tablet.</param>
        /// <returns>The settings for the tablet.</returns>
        public BindableProfile GetSettingsForTablet(string name)
        {
            var tablet = _tablets.Find(t => t.Name == name);

            if (tablet == null)
                return null!;

            return TouchGestureSettings?.Profiles.Find(p => p.Name == tablet.Name) ?? CreateProfileForTablet(tablet);
        }

        public void AddTablet(SharedTabletReference tablet)
        {
            if (_tablets.Any(t => t.Name == tablet.Name))
                return;

            _tablets.Add(tablet);

            TabletsChanged?.Invoke(this, _tablets);
            TabletAdded?.Invoke(this, tablet);
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

        private BindableProfile CreateProfileForTablet(SharedTabletReference tablet)
        {
            if (TouchGestureSettings == null)
                return null!;

            var profile = new BindableProfile
            {
                Name = tablet.Name
            };

            TouchGestureSettings.Profiles.Add(profile);

            return profile;
        }

        private void IdentifyPlugins()
        {
            // Obtain all IBinding plugins
            var plugins = OpenTabletDriver.Desktop.AppInfo.PluginManager.GetChildTypes<IBinding>();

            IdentifierToPluginConversion.Clear();

            foreach (var plugin in plugins)
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

        private void LoadSettings(string path)
        {
            if (!File.Exists(path))
                SaveSettingsCore();

            HasErrored = !Settings.TryLoadFrom(path, out var temp);

            if (!HasErrored)
                TouchGestureSettings = temp;
        }

        private bool SaveSettingsCore()
        {
            Log.Write("Gestures Daemon", "Saving settings...");

            try
            {
                File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(TouchGestureSettings, _serializerSettings));
                return true;
            }
            catch (Exception e)
            {
                Log.Write("Gestures Daemon", $"Failed to save settings: {e.Message}", LogLevel.Error);
            }

            return false;
        }

        #endregion

        #region RPC Methods

        public Task<List<SerializablePlugin>> GetPlugins()
        {
            Log.Write("Gestures Daemon", "Getting plugins...");

            List<SerializablePlugin> plugins = new();

            foreach (var IdentifierPluginPair in IdentifierToPluginConversion)
            {
                var plugin = IdentifierPluginPair.Value;

                var store = new PluginSettingStore(plugin);

                var validateBinding = store.Construct<IValidateBinding>();

                var serializablePlugin = new SerializablePlugin(plugin.GetCustomAttribute<PluginNameAttribute>()?.Name,
                                                                plugin.FullName,
                                                                IdentifierPluginPair.Key,
                                                                validateBinding.ValidProperties);

                plugins.Add(serializablePlugin);
            }

            return Task.FromResult(plugins);
        }

        public Task<bool> IsTabletConnected()
        {
            Log.Write("Gestures Daemon", "Checking if tablet is connected...");

            return Task.FromResult(Info.Driver.Tablet != null);
        }

        public Task<SharedTabletReference[]> GetTablets()
        {
            if (Info.Driver.Tablet == null)
                return Task.FromResult(Array.Empty<SharedTabletReference>());

            Log.Write("Gestures Daemon", "Getting tablets...");

            var tablets = new SharedTabletReference[1] { Info.Driver.Tablet.ToShared() };

            return Task.FromResult(tablets);
        }

        public Task<Vector2> GetTabletSize()
        {
            Log.Write("Gestures Daemon", "Acquiring Tablet...");

            return Task.FromResult(TabletSize);
        }

        public Task<Vector2> GetTabletLinesPerMM()
        {
            Log.Write("Gestures Daemon", "Acquiring Tablet...");

            return Task.FromResult(TouchLPMM);
        }

        public Task<SerializableSettings> GetSettings()
        {
            Log.Write("Gestures Daemon", "Converting Settings into a serializable form...");

            if (TouchGestureSettings == null)
                return Task.FromResult<SerializableSettings>(null!);

            var serializedSettings = Settings.ToSerializable(TouchGestureSettings, IdentifierToPluginConversion);

            return Task.FromResult(serializedSettings);
        }

        public Task<bool> SaveSettings() => Task.FromResult(SaveSettingsCore());

        public Task<bool> UpdateSettings(SerializableSettings settings)
        {
            Log.Write("Gestures Daemon", "Updating settings...");

            if (settings == null)
                return Task.FromResult(false);

            TouchGestureSettings = Settings.FromSerializable(settings, IdentifierToPluginConversion);
            OnSettingsChanged?.Invoke(this, TouchGestureSettings);

            return Task.FromResult(true);
        }

        public Task<bool> UpdateProfile(SerializableProfile profile)
        {
            Log.Write("Gestures Daemon", "Updating profile...");

            if (profile == null)
                return Task.FromResult(false);

            var bindableProfile = TouchGestureSettings?.Profiles.Find(p => p.Name == profile.Name);
            var tablet = _tablets.Find(t => t.Name == profile.Name);

            if (bindableProfile == null || tablet == null)
                return Task.FromResult(false);

            bindableProfile.Update(profile, tablet, IdentifierToPluginConversion);

            return Task.FromResult(true);
        }

        public Task<bool> StartRecording()
        {
            return Task.FromResult(true);
        }

        public Task<bool> StopRecording()
        {
            return Task.FromResult(true);
        }

        #endregion

        #region Event Handlers

        private void OnTabletsAdded(object? sender, SharedTabletReference tablet)
        {
            TabletSize = _tablets[0].Size;
            TouchLPMM = _tablets[0].TouchDigitizer?.GetLPMM() ?? Vector2.One;

            if (TouchGestureSettings == null)
                return;

            var profile = TouchGestureSettings.Profiles.Find(p => p.Name == _tablets[0].Name);

            if (profile == null)
                return;

            // Update gestures in profile with new LPMM
            foreach (var gesture in profile)
            {
                gesture.LinesPerMM = TouchLPMM;
            }
        }

        #endregion

        #region Disposal

        public void Dispose()
        {
            _tablets.Clear();
            _rpcServer.Dispose();
        }

        #endregion
    }
}
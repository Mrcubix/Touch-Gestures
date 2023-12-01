using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using TouchGestures.Entities;
using TouchGestures.Lib.Contracts;
using TouchGestures.Lib.Entities;
using WheelAddon.Converters;

namespace TouchGestures
{
    public class GesturesDaemon : IGesturesDaemon
    {
        #region Constants

        private readonly string _settingsPath = Path.Combine(OpenTabletDriver.Desktop.AppInfo.Current.AppDataDirectory, "Touch-Gestures.json");

        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new PluginSettingStoreConverter(),
                new PluginSettingConverter()
            }
        };

        #endregion

        #region Constructors

        public GesturesDaemon()
        {
        }

        public GesturesDaemon(Settings settings)
        {
            TouchGestureSettings = settings;

            Initialize(false);
        }

        public void Initialize(bool doLoadSettings = true)
        {
            if (doLoadSettings)
                LoadSettings(_settingsPath);

            if (HasErrored)
            {
                Log.Write("Gestures Daemon", "Failed to load settings, aborting initialization.");
                return;
            }

            if (Info.Driver.Tablet != null)
            {
                TabletSize = new Vector2(Info.Driver.Tablet.Digitizer.Width, Info.Driver.Tablet.Digitizer.Height);
                LinesPerMM = Info.Driver.Tablet.Digitizer.MaxX / Info.Driver.Tablet.Digitizer.Width;
            }

            // identify plugins
            IdentifyPlugins();

            // construct bindings
            TouchGestureSettings.ConstructBindings();

            OnSettingsChanged?.Invoke(this, TouchGestureSettings);

            IsReady = true;
            OnReady?.Invoke(this, null!);
        }

        #endregion

        #region Events

        public event EventHandler? OnReady;
        public event EventHandler<Settings>? OnSettingsChanged;

        #endregion

        #region Properties

        public Dictionary<int, TypeInfo> IdentifierToPluginConversion = new();

        public Settings TouchGestureSettings { get; private set; } = Settings.Default;

        public Vector2 TabletSize { get; private set; } = new(-1, -1);

        public float LinesPerMM = -1;

        public bool IsReady { get; private set; }

        public bool HasErrored { get; private set; }

        #endregion

        #region Methods

        private void IdentifyPlugins()
        {
            // obtain all IBinding plugins
            var plugins = OpenTabletDriver.Desktop.AppInfo.PluginManager.GetChildTypes<IBinding>();

            foreach(var plugin in plugins)
            {
                var identifierHash = 0;

                if (plugin.FullName == null)
                    continue;

                for (var i = 0; i < plugin.FullName.Length; i++)
                    identifierHash += plugin.FullName[i];

                IdentifierToPluginConversion.Add(identifierHash, plugin);
            }
        }

        private void LoadSettings(string path)
        {
            if (!File.Exists(_settingsPath))
                SaveSettings();

            var res = Settings.TryLoadFrom(_settingsPath, out var temp);

            if (res)
                TouchGestureSettings = temp;
            else
                HasErrored = true;
        }

        private bool SaveSettings()
        {
            Log.Write("Gesture Daemon", "Saving settings...");

            try
            {
                File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(TouchGestureSettings, _serializerSettings));
                return true;
            }
            catch (Exception e)
            {
                Log.Write("Gesture Daemon", $"Failed to save settings: {e.Message}", LogLevel.Error);
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

        public Task<Vector2> GetTabletSize()
        {
            Log.Write("Gestures Daemon", "Acquiring Tablet...");

            return Task.FromResult(TabletSize);
        }

        public Task<float> GetTabletLinesPerMM()
        {
            Log.Write("Gestures Daemon", "Acquiring Tablet...");

            return Task.FromResult(LinesPerMM);
        }

        public Task<SerializableSettings> GetSettings()
        {
            Log.Write("Gestures Daemon", "Converting Settings into a serializable form...");

            if (TouchGestureSettings == null)
                return Task.FromResult<SerializableSettings>(null!);

            var serializedSettings = Settings.ToSerializable(TouchGestureSettings, IdentifierToPluginConversion);

            return Task.FromResult(serializedSettings);
        }

        public Task<bool> UpdateSettings(SerializableSettings settings)
        {
            Log.Write("Gestures Daemon", "Updating settings...");

            if (settings == null)
                return Task.FromResult(false);

            TouchGestureSettings = Settings.FromSerializable(settings, IdentifierToPluginConversion);
            OnSettingsChanged?.Invoke(this, TouchGestureSettings);

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
    }
}
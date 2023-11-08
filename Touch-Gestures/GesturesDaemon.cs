using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using TouchGestures.Entities.Serializables;
using TouchGestures.Lib.Contracts;
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

        public bool IsReady { get; private set; }

        public bool HasErrored { get; private set; }

        public Settings TouchGestureSettings { get; private set; } = Settings.Default;

        public Dictionary<int, TypeInfo> IdentifierToPluginConversion = new();

        #endregion

        #region Methods

        private void IdentifyPlugins()
        {
            // obtain all IBinding plugins
            var plugins = OpenTabletDriver.Desktop.AppInfo.PluginManager.GetChildTypes<IBinding>();

            foreach(var plugin in plugins)
            {
                var identifierHash = 0;

                for (var i = 0; i < plugin.Name.Length; i++)
                    identifierHash += (plugin.Name[i] * i) + 1;

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
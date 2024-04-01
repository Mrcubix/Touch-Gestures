using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin;
using System.Reflection;
using TouchGestures.Lib.Converters;

namespace TouchGestures.Lib.Entities
{
    public class Settings
    {
        #region Constants
        
        private static readonly JsonSerializerSettings _serializerSettings = new()
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

        #region Constructors

        [JsonConstructor]
        public Settings()
        {
        }

        #endregion

        #region Properties

        [JsonProperty]
        public int Version { get; set; }

        [JsonProperty]
        public List<BindableProfile> Profiles { get; set; } = new();

        #endregion

        #region Methods

        public static bool TryLoadFrom(string path, out Settings? settings)
        {
            settings = null!;

            if (File.Exists(path))
            {
                try
                {
                    var serialized = File.ReadAllText(path);
                    settings = JsonConvert.DeserializeObject<Settings>(serialized, _serializerSettings)!;

                    return true;
                }
                catch(Exception e)
                {
                    Log.Write("Touch Gestures Settings Loader", $"Failed to load settings from {path}: {e}", LogLevel.Error);
                }
            }
            else
            {
                Log.Write("Touch Gestures Settings Loader", $"Failed to load settings from {path}: file does not exist", LogLevel.Error);
            }

            return false;
        }

        /// <summary>
        ///   Some bindings may not have a pointer provided to them using this method.
        /// </summary>
        public void ConstructBindings()
        {
            foreach (var profile in Profiles)
                profile.ConstructBindings();
        }

        #endregion

        #region Static Properties

        public static Settings Default => new();

        #endregion

        #region Static Methods

        public static Settings FromSerializable(SerializableSettings settings, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            var result = new Settings();

            foreach (var profile in settings.Profiles)
                if (BindableProfile.FromSerializable(profile, identifierToPlugin) is BindableProfile bindableProfile)
                    result.Profiles.Add(bindableProfile);

            return result;
        }

        public static SerializableSettings ToSerializable(Settings settings, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            var result = new SerializableSettings();

            foreach (var profile in settings.Profiles)
                if (BindableProfile.ToSerializable(profile, identifierToPlugin) is SerializableProfile serializableProfile)
                    result.Profiles.Add(serializableProfile);

            return result;
        }

        #endregion
    }
}
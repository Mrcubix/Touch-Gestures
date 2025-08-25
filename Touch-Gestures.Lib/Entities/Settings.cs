using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Converters.Json;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Lib.Entities
{
    public class Settings
    {
        #region Constants

        public static readonly List<JsonConverter> Converters = new()
        {
            new SerializablePluginSettingsConverter(),
            new SharedAreaConverter()
        };

        private static readonly JsonSerializerSettings _serializerSettings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = Converters
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
        public int Version { get; set; } = 3;

        [JsonProperty]
        public List<GestureProfile> Profiles { get; set; } = new();

        #endregion

        #region Methods

        public void UpdateFrom(Settings other, List<SharedTabletReference> tablets)
        {
            Version = other.Version;

            // Update each profiles
            foreach (var profile in Profiles)
            {
                var bindableProfile = other?.Profiles.Find(p => p.Name == profile.Name);
                var tablet = tablets.Find(t => t.Name == profile.Name);

                if (bindableProfile == null || tablet == null)
                    return;

                bindableProfile.Update(profile, tablet);
            }
        }

        /// <summary>
        ///   Some bindings may not have a pointer provided to them using this method.
        /// </summary>
        public void ConstructBindings(SharedTabletReference tablet)
        {
            foreach (var profile in Profiles)
                profile.ConstructBindings(tablet);
        }

        #endregion

        #region Static Properties

        public static Settings Default => new();

        #endregion

        #region Static Methods

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
                catch (Exception e)
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

        public static bool TrySaveTo(string path, in Settings settings)
        {
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(settings, _serializerSettings)!);
                return true;
            }
            catch (Exception e)
            {
                Log.Write("Touch Gestures Settings Loader", $"Failed to save settings to {path}: {e}", LogLevel.Error);
                return false;
            }
        }

        #endregion
    }
}
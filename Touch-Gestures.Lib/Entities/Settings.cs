using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Reflection;
using TouchGestures.Lib.Enums;

namespace TouchGestures.Lib.Entities
{
    public class Settings
    {
        #region Constants
        
        public static readonly JsonSerializerSettings SerializerSettings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new BindingSettingConverter(),
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

        #endregion

        #region Methods

        public virtual bool TryLoadFrom(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var serialized = File.ReadAllText(path);
                    Settings settings = JsonConvert.DeserializeObject<Settings>(serialized, SerializerSettings)!;

                    Version = settings.Version;

                    return true;
                }
                catch(Exception e)
                {
                    Logger.Instance?.Write("Touch Gestures Settings Loader", $"Failed to load settings from {path}: {e}", LogLevel.Error);
                }
            }
            else
            {
                Logger.Instance?.Write("Touch Gestures Settings", $"Failed to load settings from {path}: file does not exist", LogLevel.Error);
            }

            return false;
        }

        public virtual bool TrySaveTo(string path)
        {
            Logger.Instance?.Write("Gestures Daemon", "Saving settings...");

            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(this, SerializerSettings));
                return true;
            }
            catch(Exception e)
            {
                Logger.Instance?.Write("Touch Gestures Settings", $"Failed to save settings: {e.Message}", LogLevel.Error);
            }

            return false;
        }

        public static bool TryLoadFrom<TProfile, TStore>(string path, out Settings<TProfile, TStore>? settings)
            where TProfile : BindableProfile, new()
            where TStore : BindingSettingStore, new()
        {
            settings = null!;

            if (File.Exists(path))
            {
                try
                {
                    var serialized = File.ReadAllText(path);
                    settings = JsonConvert.DeserializeObject<Settings<TProfile, TStore>>(serialized, SerializerSettings)!;

                    return true;
                }
                catch(Exception e)
                {
                    Logger.Instance?.Write("Touch Gestures Settings Loader", $"Failed to load settings from {path}: {e}", LogLevel.Error);
                }
            }
            else
            {
                Logger.Instance?.Write("Touch Gestures Settings", $"Failed to load settings from {path}: file does not exist", LogLevel.Error);
            }

            return false;
        }

        public static bool TrySaveTo(string path, Settings settings)
        {
            if (settings == null)
                return false;

            Logger.Instance?.Write("Gestures Daemon", "Saving settings...");

            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(settings, SerializerSettings));
                return true;
            }
            catch(Exception e)
            {
                Logger.Instance?.Write("Touch Gestures Settings", $"Failed to save settings: {e.Message}", LogLevel.Error);
            }

            return false;
        }

        public virtual SerializableSettings ToSerializable(Dictionary<int, TypeInfo> identifierToPlugin)
            => throw new NotSupportedException();

        /// <summary>
        ///   Some bindings may not have a pointer provided to them using this method.
        /// </summary>
        public virtual void ConstructBindings() {}

        #endregion

        #region Static Properties

        public static Settings Default => new();

        #endregion
    }

    public class Settings<TProfile, TStore> : Settings
        where TProfile : BindableProfile, new()
        where TStore : BindingSettingStore, new()
    {
        public Settings(SerializableSettings settings, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            foreach (var profile in settings.Profiles)
                if (BindableProfile.FromSerializable<TProfile, TStore>(profile, identifierToPlugin) is TProfile bindableProfile)
                    Profiles.Add(bindableProfile);
        }

        public Settings(string path)
        {
            if (TryLoadFrom(path))
                return;
        }

        [JsonConstructor]
        public Settings()
        {
        }

        [JsonProperty]
        public List<TProfile> Profiles { get; set; } = new();

        public override void ConstructBindings()
        {
            foreach (var profile in Profiles)
                profile.ConstructBindings();
        }

        public override SerializableSettings ToSerializable(Dictionary<int, TypeInfo> identifierToPlugin)
        {
            var result = new SerializableSettings();

            foreach (var profile in Profiles)
                if (BindableProfile.ToSerializable(profile, identifierToPlugin) is SerializableProfile serializableProfile)
                    result.Profiles.Add(serializableProfile);

            return result;
        }

        public static Settings<TProfile, TStore> FromSerializable(SerializableSettings settings, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            return new Settings<TProfile, TStore>(settings, identifierToPlugin);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin;
using WheelAddon.Converters;
using System.Reflection;
using TouchGestures.Lib.Entities;
using TouchGestures.Entities.Gestures;

namespace TouchGestures.Entities.Serializables
{
    public class Settings
    {
        #region Constants

        private static readonly JsonSerializer _serializer = new()
        {
            Formatting = Formatting.Indented,
        };
        
        private static readonly JsonSerializerSettings _serializerSettings = new()
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

        [JsonConstructor]
        public Settings()
        {
        }

        #endregion

        #region Properties

        [JsonProperty]
        public List<BindableTapGesture> TapGestures { get; set; } = new();

        [JsonProperty]
        public List<BindableSwipeGesture> SwipeGestures { get; set; } = new();

        #endregion

        #region Methods

        public static bool TryLoadFrom(string path, out Settings settings)
        {
            settings = null!;

            if (File.Exists(path))
            {
                try
                {
                    settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path), _serializerSettings)!;

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

        public void ConstructBindings()
        {

        }

        /*public SerializableSettings ToSerializable(Dictionary<int, TypeInfo> identifierToPlugin)
        {
            
        }*/

        #endregion

        #region Static Properties

        public static Settings Default => new();

        #endregion

        #region Static Methods

        public static Settings FromSerializable(SerializableSettings settings, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            var result = new Settings();

            foreach (var gesture in settings.TapGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                var tapGesture = BindableTapGesture.FromSerializable(gesture, identifierToPlugin);

                if (tapGesture != null)
                    result.TapGestures.Add(tapGesture);
            }

            foreach (var gesture in settings.SwipeGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                var swipeGesture = BindableSwipeGesture.FromSerializable(gesture, identifierToPlugin);

                if (swipeGesture != null)
                    result.SwipeGestures.Add(swipeGesture);
            }

            // TODO: Implement the rest of the gestures conversions

            return result;
        }

        #endregion
    }
}
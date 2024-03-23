using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin;
using System.Reflection;
using TouchGestures.Converters;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Entities;
using TouchGestures.Entities.Gestures;

namespace TouchGestures.Entities
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
        public List<BindableTapGesture> TapGestures { get; set; } = new();

        [JsonProperty]
        public List<BindableHoldGesture> HoldGestures { get; set; } = new();

        [JsonProperty]
        public List<BindableSwipeGesture> SwipeGestures { get; set; } = new();

        [JsonProperty]
        public List<BindablePanGesture> PanGestures { get; set; } = new();

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

        public void ConstructBindings()
        {
            foreach (var gesture in TapGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>();

            foreach (var gesture in SwipeGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>();

            foreach (var gesture in HoldGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>();

            foreach (var gesture in PanGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>();
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

            foreach (var gesture in settings.HoldGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                var holdGesture = BindableHoldGesture.FromSerializable(gesture, identifierToPlugin);

                if (holdGesture != null)
                    result.HoldGestures.Add(holdGesture);
            }

            foreach (var gesture in settings.SwipeGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                var swipeGesture = BindableSwipeGesture.FromSerializable(gesture, identifierToPlugin);

                if (swipeGesture != null)
                    result.SwipeGestures.Add(swipeGesture);
            }

            foreach (var gesture in settings.PanGestures)
            {
                if (gesture.PluginProperty == null)
                    continue;

                var panGesture = BindablePanGesture.FromSerializable(gesture, identifierToPlugin);

                if (panGesture != null)
                    result.PanGestures.Add(panGesture);
            }

            // TODO: Implement the rest of the gestures conversions

            return result;
        }

        public static SerializableSettings ToSerializable(Settings settings, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            var result = new SerializableSettings();

            foreach (var gesture in settings.TapGestures)
            {
                var tapGesture = BindableTapGesture.ToSerializable(gesture, identifierToPlugin);

                if (tapGesture != null)
                    result.TapGestures.Add(tapGesture);
            }

            foreach (var gesture in settings.HoldGestures)
            {
                var holdGesture = BindableHoldGesture.ToSerializable(gesture, identifierToPlugin);

                if (holdGesture != null)
                    result.HoldGestures.Add(holdGesture);
            }

            foreach (var gesture in settings.SwipeGestures)
            {
                var swipeGesture = BindableSwipeGesture.ToSerializable(gesture, identifierToPlugin);

                if (swipeGesture != null)
                    result.SwipeGestures.Add(swipeGesture);
            }

            foreach (var gesture in settings.PanGestures)
            {
                var panGesture = BindablePanGesture.ToSerializable(gesture, identifierToPlugin);

                if (panGesture != null)
                    result.PanGestures.Add(panGesture);
            }

            return result;
        }

        #endregion
    }
}
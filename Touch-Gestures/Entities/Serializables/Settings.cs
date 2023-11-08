using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin;
using WheelAddon.Converters;
using System.Reflection;

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
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTabletDriver.External.Common.Serializables;

namespace TouchGestures.Lib.Converters.Json
{
    /// <summary>
    ///   A converter for use when writing settings to file.
    /// </summary>
    public class SerializablePluginSettingsStoreConverter : JsonConverter<SerializablePluginSettingsStore>
    {
        public SerializablePluginSettingsStoreConverter(IEnumerable<SerializablePlugin> plugins)
        {
            Plugins = plugins;
        }

        #region Properties

        public IEnumerable<SerializablePlugin> Plugins { get; }

        #endregion

        #region Methods

        public override SerializablePluginSettingsStore? ReadJson(JsonReader reader, Type objectType, SerializablePluginSettingsStore? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = JObject.Load(reader);

            if (value == null)
                return null;

            var fullName = value["Path"]?.Value<string>();
            var settings = value["Settings"]?.CreateReader();
            var enable = value["Enable"]?.Value<bool>();

            if (fullName == null || settings == null || enable == null)
                return null;

            var plugin = Plugins.FirstOrDefault(p => p.FullName == fullName);

            if (plugin == null)
                return null;

            return new SerializablePluginSettingsStore()
            {
                PluginName = plugin.PluginName,
                FullName = fullName,
                Identifier = plugin.Identifier,
                Settings = serializer.Deserialize<ObservableCollection<SerializablePluginSettings>>(settings) ?? new(),
                Enable = enable ?? true
            };
        }

        public override void WriteJson(JsonWriter writer, SerializablePluginSettingsStore? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var obj = new JObject
            {
                ["Path"] = value.FullName,
                ["Settings"] = JToken.FromObject(value.Settings, serializer),
                ["Enable"] = value.Enable
            };

            obj.WriteTo(writer);
        }

        #endregion
    }
}
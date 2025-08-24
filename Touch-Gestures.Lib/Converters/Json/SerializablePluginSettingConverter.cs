using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTabletDriver.External.Common.Serializables;

namespace TouchGestures.Lib.Converters.Json
{
    /// <summary>
    ///   A converter for use when writing settings to file.
    /// </summary>
    public class SerializablePluginSettingsConverter : JsonConverter<SerializablePluginSettings>
    {
        #region Methods

        public override SerializablePluginSettings? ReadJson(JsonReader reader, Type objectType, SerializablePluginSettings? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // build a PluginSettingsStore using value.Path as argument
            var value = JObject.Load(reader);

            if (value == null)
                return null;

            if (!value.TryGetValue("Property", out var property))
                return null;

            if (!value.TryGetValue("Value", out var settingValue))
                return null;

            if (property == null || settingValue == null)
                return null;

            return new SerializablePluginSettings(property.Value<string>() ?? "Unknown Property", -1, settingValue);
        }

        public override void WriteJson(JsonWriter writer, SerializablePluginSettings? value, JsonSerializer serializer)
        {
            // write a JObject with Path, Settings and Enable properties
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var obj = new JObject
            {
                ["Property"] = value.Property,
                ["Value"] = value.Value,
                ["HasValue"] = value.HasValue
            };

            obj.WriteTo(writer);
        }

        #endregion
    }
}
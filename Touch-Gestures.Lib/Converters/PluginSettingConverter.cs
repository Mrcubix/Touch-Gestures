using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTabletDriver.Desktop.Reflection;

namespace TouchGestures.Converters
{
    public class PluginSettingConverter : JsonConverter<PluginSetting>
    {
        public override PluginSetting? ReadJson(JsonReader reader, Type objectType, PluginSetting? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // build a PluginSettingsStore using value.Path as argument
            var value = JObject.Load(reader);

            if (value == null)
                return null!;

            if (!value.TryGetValue("Property", out var property))
                return null!;

            if (!value.TryGetValue("Value", out var settingValue))
                return null!;

            if (property == null || settingValue == null)
                return null!;

            return new PluginSetting(property?.Value<string>()!, settingValue?.Value<string>()!);
        }

        public override void WriteJson(JsonWriter writer, PluginSetting? value, JsonSerializer serializer)
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
                ["Value"] = value.GetValue<string?>(),
#if !OTD06
                ["HasValue"] = value.HasValue
#endif
            };

            obj.WriteTo(writer);
        }
    }
}
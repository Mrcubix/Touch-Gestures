using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin;

namespace TouchGestures.Lib.Converters
{
    public class PluginSettingStoreConverter : JsonConverter<PluginSettingStore>
    {
        private readonly IReadOnlyCollection<TypeInfo> pluginsTypes = AppInfo.PluginManager.PluginTypes;

        public override PluginSettingStore? ReadJson(JsonReader reader, Type objectType, PluginSettingStore? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // build a PluginSettingsStore using value.Path as argument
            var value = JObject.Load(reader);

            if (value == null)
                return null!;

            var path = value["Path"]?.Value<string>();
            var type = pluginsTypes.FirstOrDefault(t => t.FullName == path);

            if (type == null)
            {
                Log.Write("Wheel Handler Settings Converter", $"Plugin {path} not found", LogLevel.Error);
                return null!;
            }

            var pluginSettingsReader = value["Settings"]?.CreateReader();

            if (pluginSettingsReader == null)
            {
                Log.Write("Wheel Handler Settings Converter", $"Plugin {path} settings not found", LogLevel.Error);
                return null!;
            }

            ObservableCollection<PluginSetting> settings = serializer.Deserialize<ObservableCollection<PluginSetting>>(pluginSettingsReader) 
                                                               ?? new ObservableCollection<PluginSetting>(); 

            var enabled = value["Enable"]?.Value<bool>();

            var store = new PluginSettingStore(type)
            {
                Settings = settings,
                Enable = enabled ?? false
            };

            return store;
        }

        public override void WriteJson(JsonWriter writer, PluginSettingStore? value, JsonSerializer serializer)
        {
            // write a JObject with Path, Settings and Enable properties
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var obj = new JObject
            {
                ["Path"] = value.Path,
                ["Settings"] = JToken.FromObject(value.Settings, serializer),
                ["Enable"] = value.Enable
            };

            obj.WriteTo(writer, serializer.Converters.ToArray());
        }
    }
}
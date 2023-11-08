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

namespace WheelAddon.Converters
{
    public class PluginSettingStoreConverter : JsonConverter
    {
        private readonly IReadOnlyCollection<TypeInfo> pluginsTypes = AppInfo.PluginManager.PluginTypes;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PluginSettingStore);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
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

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            // write a JObject with Path, Settings and Enable properties
            var store = (PluginSettingStore?)value;

            // if store is null, write null
            if (store == null)
            {
                writer.WriteNull();
                return;
            }

            var obj = new JObject
            {
                ["Path"] = store.Path,
                ["Settings"] = JToken.FromObject(store.Settings, serializer),
                ["Enable"] = store.Enable
            };

            obj.WriteTo(writer, serializer.Converters.ToArray());
        }
    }
}
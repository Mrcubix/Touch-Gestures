using System;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Plugin;
using TouchGestures.Entities;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Reflection;

namespace TouchGestures.Converters
{
    public class StableBindingSettingStoreConverter : BindingSettingStoreConverter
    {
        public StableBindingSettingStoreConverter() : base(AppInfo.PluginManager.PluginTypes) { }

        public override BindingSettingStore? ReadJson(JsonReader reader, Type objectType, BindingSettingStore? existingValue, 
                                                      bool hasExistingValue, JsonSerializer serializer)
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

            ObservableCollection<BindingSetting> settings = serializer.Deserialize<ObservableCollection<BindingSetting>>(pluginSettingsReader)
                                                               ?? new ObservableCollection<BindingSetting>();

            var enabled = value["Enable"]?.Value<bool>();

            var store = new StableBindingSettingStore(type)
            {
                Settings = settings,
                Enable = enabled ?? false
            };

            return store;
        }

        public override void WriteJson(JsonWriter writer, BindingSettingStore? value, JsonSerializer serializer)
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
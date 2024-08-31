using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin.Attributes;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Reflection;
using TouchGestures.Extensions;

namespace TouchGestures.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StableBindingSettingStore : BindingSettingStore
    {
        private PluginSettingStore _internalStore = null!;

        public StableBindingSettingStore(Type type, bool enable = true)
        {
            _internalStore = new PluginSettingStore(type, enable);
        }

        public StableBindingSettingStore(object source, bool enable = true)
        {
            _internalStore = new PluginSettingStore(source, enable);
        }
        
        public StableBindingSettingStore(PluginSettingStore store)
        {
            _internalStore = store;
        }

        [JsonConstructor]
        public StableBindingSettingStore()
        {
            _internalStore = new PluginSettingStore(null!);
        }

        public override string Path
        {
            get => _internalStore.Path;
            set => _internalStore.Path = value;
        }

        [JsonIgnore]
        public override string Name => throw new NotSupportedException();

        public override ObservableCollection<BindingSetting> Settings
        {
            get => _internalStore.Settings.ToShared();
            set => _internalStore.Settings = value.FromShared();
        }

        public override bool Enable 
        {
            get => _internalStore.Enable;
            set => _internalStore.Enable = value;
        }

        public PluginReference GetPluginReference() => new StoredPluginReference(AppInfo.PluginManager, _internalStore);

        public override T Construct<T>(SharedTabletReference tabletReference = null!, bool trigger = true) where T : class
        {
            return GetPluginReference().Construct<T>();
        }

        public override T Construct<T>(IServiceManager? provider, SharedTabletReference tabletReference = null!) where T : class
            => throw new NotSupportedException();

        public override void ApplySettings(object target)
        {
            _internalStore.ApplySettings(target);
        }

        public override string GetHumanReadableString()
        {
            var name = Name;
            string settings = string.Join(", ", Settings.Select(s => $"({s.Property}: {s.Value})"));
            string suffix = Settings.Any() ? $": {settings}" : string.Empty;
            return name + suffix;
        }

        public override TypeInfo GetTypeInfo()
        {
            return AppInfo.PluginManager.PluginTypes.FirstOrDefault(t => t.FullName == Path)!;
        }

        public override TypeInfo GetTypeInfo<T>()
        {
            return AppInfo.PluginManager.GetChildTypes<T>().FirstOrDefault(t => t.FullName == Path)!;
        }

        public override string GetValue(TypeInfo plugin)
        {
            if (_internalStore.Settings.Any())
            {
                if (_internalStore.Settings.Count == 1)
                    return _internalStore?.Settings[0].GetValue<string>()!;
                else
                    return _internalStore?.Settings.FirstOrDefault(x => x.Property == "Property")?.GetValue<string>()!;
            }

            return null!;
        }

        public override void SetTypeInfo(TypeInfo typeInfo)
        {
            _internalStore = new PluginSettingStore(typeInfo);
        }
        
        public override void SetSource(object source)
        {
            _internalStore = new PluginSettingStore(source);
        }

        public override bool SetValue(TypeInfo plugin, string? value)
        {
            if (_internalStore.Settings.Any())
            {
                if (_internalStore.Settings.Count == 1)
                {
                    _internalStore.Settings[0].SetValue(value!);
                    return true;
                }
                else
                {
                    _internalStore.Settings.Single(s => s.Property == "Property").SetValue(value!);
                    return true;
                }
            }

            return false;
        }

        public PluginSettingStore GetUnderlyingStore() => _internalStore;

        private static ObservableCollection<PluginSetting> GetSettingsForType(Type targetType, object source = null!)
        {
            var settings = from property in targetType.GetProperties()
                           where property.GetCustomAttribute<PropertyAttribute>() is PropertyAttribute
                           select new PluginSetting(property, source == null ? null! : property.GetValue(source)!);
            return new ObservableCollection<PluginSetting>(settings);
        }
    }
}

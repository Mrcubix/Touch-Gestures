using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin.Attributes;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Extensions;
using TouchGestures.Lib.Reflection;
using TouchGestures.Extensions;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Tablet;
using IServiceManager = TouchGestures.Lib.Reflection.IServiceManager;

namespace TouchGestures.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class BulletproofBindingSettingsStore : BindingSettingStore
    {
        private PluginSettingStore _internalStore = null!;

        private static readonly Type _tabletRefType = typeof(TabletReference);

        public BulletproofBindingSettingsStore(Type type, bool enable = true)
        {
            _internalStore = new PluginSettingStore(type, enable);
        }

        public BulletproofBindingSettingsStore(object source, bool enable = true)
        {
            _internalStore = new PluginSettingStore(source, enable);
        }

        public BulletproofBindingSettingsStore(PluginSettingStore store)
        {
            _internalStore = store;
        }

        [JsonConstructor]
        public BulletproofBindingSettingsStore()
        {
            _internalStore = new PluginSettingStore(null);
        }

        public override string Path
        {
            get => _internalStore.Path;
            set => _internalStore.Path = value;
        }

        [JsonIgnore]
        public override string Name
        {
            get => _internalStore.Name;
        }

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

        public override T Construct<T>(SharedTabletReference tabletReference = null!, bool trigger = true) where T : class
        {
            var obj = AppInfo.PluginManager.ConstructObject<T>(Path);
            ApplySettings(obj);

            if (trigger)
                TriggerEventMethods(obj, tabletReference);

            return obj;
        }

        public override T Construct<T>(IServiceManager? provider, SharedTabletReference tabletReference = null!) where T : class
        {
            var obj = Construct<T>(tabletReference, false);

            if (provider != null)
                PluginManager.Inject(provider, obj);

            TriggerEventMethods(obj, tabletReference);
            return obj;
        }

        public static PluginSettingStore FromPath(string path)
        {
            var pathType = AppInfo.PluginManager.PluginTypes.FirstOrDefault(t => t.FullName == path);
            return pathType != null ? new PluginSettingStore(pathType) : null!;
        }

        public override void ApplySettings(object target)
        {
            _internalStore.ApplySettings(target);
        }

        public override string GetHumanReadableString()
        {
            var name = Name;
            string settings = string.Join(", ", this.Settings.Select(s => $"({s.Property}: {s.Value})"));
            string suffix = Settings.Any() ? $": {settings}" : string.Empty;
            return name + suffix;
        }

        public override TypeInfo GetTypeInfo()
        {
            return _internalStore.GetTypeInfo();
        }

        public override TypeInfo GetTypeInfo<T>()
        {
            return _internalStore.GetTypeInfo<T>();
        }

        public override string GetValue(TypeInfo plugin)
        {
            if (_internalStore.Settings.Any())
            {
                if (_internalStore.Settings.Count == 1)
                    return _internalStore?.Settings[0].GetValue<string>()!;
                else
                {
                    // 0.6 so smart that you have to check for 45454 properties just to get a plugin properly
                    // So now we have to look for a property that has both the PropertyAttribute and the PropertyValidatedAttribute

                    var valueProperty = plugin.FindPropertyWithAttribute<PropertyAttribute>();
                    var validatedProperty = plugin.FindPropertyWithAttribute<PropertyValidatedAttribute>();

                    if (valueProperty == null || validatedProperty == null)
                        return null!;

                    // surely they are the same property
                    if (valueProperty != validatedProperty)
                        return null!;

                    return Settings.FirstOrDefault(x => x.Property == valueProperty.Name)?.GetValue<string>()!;
                }
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
                    // 0.6 so smart that you have to check for 45454 properties just to set a plugin properly
                    // So now we have to look for a property that has both the PropertyAttribute and the PropertyValidatedAttribute
                    
                    var valueProperty = plugin.FindPropertyWithAttribute<PropertyAttribute>();
                    var validatedProperty = plugin.FindPropertyWithAttribute<PropertyValidatedAttribute>();

                    if (valueProperty == null || validatedProperty == null)
                        return false;

                    // surely they are the same property
                    if (valueProperty != validatedProperty)
                        return false;

                    _internalStore.Settings.Single(s => s.Property == valueProperty.Name).SetValue(value);
                    return true;
                }
            }

            return false;
        }

        public PluginSettingStore GetUnderlyingStore() => _internalStore;

        private static void TriggerEventMethods(object obj, SharedTabletReference tabletReference)
        {
            if (obj == null)
                return;

            var properties = from property in obj.GetType().GetProperties()
                             let attr = property.GetCustomAttribute<TabletReferenceAttribute>()
                             where attr != null && property.PropertyType == _tabletRefType
                             select property;

            foreach (var property in properties)
                property.SetValue(obj, tabletReference.ToState());

            var methods = from method in obj.GetType().GetMethods()
                          let attr = method.GetCustomAttribute<OnDependencyLoadAttribute>()
                          where attr != null
                          select method;

            foreach (var method in methods)
                method.Invoke(obj, Array.Empty<object>());
        }
    }
}

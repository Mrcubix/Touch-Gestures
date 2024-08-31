using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using TouchGestures.Lib.Entities.Tablet;

#nullable disable

namespace TouchGestures.Lib.Reflection
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BindingSettingStore
    {
        [JsonConstructor]
        public BindingSettingStore() 
        {
        }

        public virtual string Path { set; get; }

        [JsonIgnore]
        public virtual string Name { get; }

        public virtual ObservableCollection<BindingSetting> Settings { set; get; }

        public virtual bool Enable { set; get; }

        public abstract T Construct<T>(SharedTabletReference tabletReference = null!, bool trigger = true) where T : class;

        public abstract T Construct<T>(IServiceManager provider, SharedTabletReference tabletReference = null!) where T : class;

        public abstract void ApplySettings(object target);

        public BindingSetting this[string propertyName] 
        {
            set
            {
                if (Settings.FirstOrDefault(t => t.Property == propertyName) is BindingSetting setting)
                {
                    Settings.Remove(setting);
                    Settings.Add(value);
                }
                else
                {
                    Settings.Add(value);
                }
            }
            get
            {
                var result = Settings.FirstOrDefault(s => s.Property == propertyName);
                if (result == null)
                {
                    var newSetting = new BindingSetting(propertyName, null);
                    Settings.Add(newSetting);
                    return newSetting;
                }
                return result;
            }
        }

        public BindingSetting this[PropertyInfo property]
        {
            set => this[property.Name] = value;
            get => this[property.Name];
        }

        public abstract string GetHumanReadableString();

        public abstract TypeInfo GetTypeInfo();

        public abstract TypeInfo GetTypeInfo<T>();

        public abstract void SetTypeInfo(TypeInfo typeInfo);
        
        public abstract void SetSource(object source);

        public abstract bool SetValue(TypeInfo plugin, string value);

        public abstract string GetValue(TypeInfo plugin);
    }
}

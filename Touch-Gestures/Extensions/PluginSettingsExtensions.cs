using System.Collections.ObjectModel;
using OpenTabletDriver.Desktop.Reflection;
using TouchGestures.Lib.Reflection;

#nullable disable

namespace TouchGestures.Extensions
{
    public static class PluginSettingsExtensions
    {
        public static PluginSetting FromShared(this BindingSetting setting)
        {
            return new PluginSetting(setting.Property, setting.Value);
        }

        public static BindingSetting ToShared(this PluginSetting setting)
        {
            return new BindingSetting(setting.Property, setting.Value);
        }

        public static ObservableCollection<BindingSetting> ToShared(this ObservableCollection<PluginSetting> settings)
        {
            var result = new ObservableCollection<BindingSetting>();

            foreach (var setting in settings)
                result.Add(setting.ToShared());

            return result;
        }

        public static ObservableCollection<PluginSetting> FromShared(this ObservableCollection<BindingSetting> settings)
        {
            var result = new ObservableCollection<PluginSetting>();

            foreach (var setting in settings)
                result.Add(setting.FromShared());

            return result;
        }
    }
}
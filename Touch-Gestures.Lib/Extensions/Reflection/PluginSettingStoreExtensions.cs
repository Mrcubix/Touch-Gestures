using System.Linq;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.Serializables;

namespace TouchGestures.Lib.Extensions.Reflection
{
    public static class PluginSettingStoreExtensions
    {
        internal static GesturesDaemonBase? Instance => GesturesDaemonBase.Instance;

        public static PluginSettingStore FromSerializable(SerializablePluginSettingsStore store)
        {
            return new PluginSettingStore(null, true)
            {
                Path = store.FullName,
                Settings = store.Settings.FromSerializableCollection(),
            };
        }

        public static SerializablePluginSettingsStore ToSerializable(this PluginSettingStore store)
        {
            if (Instance == null)
                return null!;

            var plugin = Instance.Plugins.FirstOrDefault(p => p.FullName == store.Path);

            return new SerializablePluginSettingsStore(plugin!)
            {
                FullName = store.Path,
                Settings = store.Settings.ToSerializableCollection(),
            };
        }
    }
}
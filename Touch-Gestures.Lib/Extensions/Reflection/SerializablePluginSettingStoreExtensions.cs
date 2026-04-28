using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.Serializables;

namespace TouchGestures.Lib.Extensions.Reflection
{
    public static class SerializablePluginSettingStoreExtensions
    {
        public static PluginSettingStore FromSerializable(this SerializablePluginSettingsStore store)
        {
            return new PluginSettingStore(new object(), true)
            {
                Path = store.FullName,
                Settings = store.Settings.FromSerializableCollection(),
            };
        }
    }
}
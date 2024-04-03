using System.Linq;
using System.Reflection;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin.Attributes;

namespace TouchGestures.Lib.Extensions
{
    public static class PluginSettingsStoreExtensions
    {
        public static string? GetBindingValue(this PluginSettingStore store, TypeInfo plugin)
        {
            if (store.Settings.Any())
            {
                if (store.Settings.Count == 1)
                {
                    return store?.Settings[0].GetValue<string?>();
                }
                else
                {
                    // 0.6 so smart that you have to check for 45454 properties just to get a plugin properly
                    // So now we have to look for a property that has both the PropertyAttribute and the PropertyValidatedAttribute
#if NET6_0
                    GetBindingValueCore06(store, plugin);
#else
                    return store?.Settings.FirstOrDefault(x => x.Property == "Property")?.GetValue<string?>();
#endif
                }
            }

            return null;
        }

#if NET6_0

        /// <summary>
        ///   Gets the value of the binding property of the plugin.
        /// </summary>
        /// <param name="store">The plugin setting store.</param>
        /// <param name="plugin">The plugin type.</param>
        /// <returns>The value of the binding property.</returns>
        /// <remarks>
        ///   This method is only available in 0.6.x.
        /// </remarks>
        private static string? GetBindingValueCore06(this PluginSettingStore store, TypeInfo plugin)
        {
            var valueProperty = plugin.FindPropertyWithAttribute<PropertyAttribute>();
            var validatedProperty = plugin.FindPropertyWithAttribute<PropertyValidatedAttribute>();

            if (valueProperty == null || validatedProperty == null)
                return null;

            // surely they are the same property
            if (valueProperty != validatedProperty)
                return null;

            return store?.Settings.FirstOrDefault(x => x.Property == valueProperty.Name)?.GetValue<string?>();
        }

#endif

        public static bool SetBindingValue(this PluginSettingStore store, TypeInfo plugin, string? value)
        {
            if (store.Settings.Any())
            {
                if (store.Settings.Count == 1)
                {
                    store.Settings[0].SetValue(value!);
                    return true;
                }
                else
                {
                    // 0.6 so smart that you have to check for 45454 properties just to set a plugin properly
                    // So now we have to look for a property that has both the PropertyAttribute and the PropertyValidatedAttribute
#if NET6_0
                    return SetBindingValueCore06(store, plugin, value);
#else
                    store.Settings.Single(s => s.Property == "Property").SetValue(value!);
#endif
                    return true;
                }
            }

            return false;
        }

#if NET6_0

        private static bool SetBindingValueCore06(this PluginSettingStore store, TypeInfo plugin, string? value)
        {
            var valueProperty = plugin.FindPropertyWithAttribute<PropertyAttribute>();
            var validatedProperty = plugin.FindPropertyWithAttribute<PropertyValidatedAttribute>();

            if (valueProperty == null || validatedProperty == null)
                return false;

            // surely they are the same property
            if (valueProperty != validatedProperty)
                return false;

            store.Settings.Single(s => s.Property == valueProperty.Name).SetValue(value);

            return true;
        }

#endif
    }
}
using TouchGestures.Lib.Entities.Tablet;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin;
using System;

namespace TouchGestures.Lib.Entities
{
    public class BindingBuilder
    {
        static BindingBuilder()
        {
            Build = BuildCore;
        }

        public static Func<PluginSettingStore?, SharedTabletReference?, object?> Build { get; protected set; }

        private static object? BuildCore(PluginSettingStore? store, SharedTabletReference? tablet)
        {
            return store?.Construct<IBinding>();
        }
    }
}
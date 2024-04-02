using System;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Entities
{
    public class BulletproofBindingBuilder : BindingBuilder
    {
        private static object? BuildCore(PluginSettingStore? store, SharedTabletReference? tablet)
        {
            if (tablet is not BulletproofSharedTabletReference btablet)
                return null;

            return store?.Construct<IBinding>(btablet.ServiceProvider);
        }

        public static void ChooseAsBuilder()
        {
            Build = BuildCore;
        }
    }
}
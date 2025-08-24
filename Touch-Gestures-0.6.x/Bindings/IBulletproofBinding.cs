using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;

namespace TouchGestures.Bindings
{
    public interface IBulletproofBinding
    {
        IServiceManager? Provider { get; set; }
        TabletReference? Tablet { get; set; }

        PluginSettingStore? Store { get; }
        IBinding? Binding { get; }
    }
}
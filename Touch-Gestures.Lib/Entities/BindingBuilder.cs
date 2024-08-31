using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Interfaces;
using TouchGestures.Lib.Reflection;

namespace TouchGestures.Lib.Entities
{
    public abstract class BindingBuilder
    {
        public static BindingBuilder? Current { get; set; }

        public IServiceManager? ServiceProvider { get; set; }

        public abstract ISharedBinding? Build(BindingSettingStore? store, SharedTabletReference? tablet);
    }
}
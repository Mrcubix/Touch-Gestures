using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Interfaces;
using TouchGestures.Lib.Reflection;

namespace TouchGestures.Entities
{
    public class BulletproofBindingBuilder : BindingBuilder
    {
        public override ISharedBinding? Build(BindingSettingStore? store, SharedTabletReference? tablet)
        {
            if (tablet is not BulletproofSharedTabletReference btablet)
                return null;

            return new BulletproofBinding(store?.Construct<IBinding>(btablet.ServiceProvider));
        }
    }
}
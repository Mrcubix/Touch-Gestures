using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Interfaces;
using TouchGestures.Lib.Reflection;

namespace TouchGestures.Entities
{
    public class StableBindingBuilder : BindingBuilder
    {
        public override ISharedBinding? Build(BindingSettingStore? store, SharedTabletReference? tablet)
        {
            if (tablet == null)
                return null;

            return new StableBinding(store?.Construct<IBinding>());
        }
    }
}
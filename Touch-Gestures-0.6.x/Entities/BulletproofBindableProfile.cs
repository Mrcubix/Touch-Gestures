using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Entities
{
    public class BulletproofBindableProfile : BindableProfile
    {
        public override void ConstructBindings(SharedTabletReference? tablet = null)
        {
            if (tablet is not BulletproofSharedTabletReference bTablet)
                return;

            foreach (var gesture in TapGestures)
                gesture.Binding = new BulletproofBinding(gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider));

            foreach (var gesture in SwipeGestures)
                gesture.Binding = new BulletproofBinding(gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider));

            foreach (var gesture in HoldGestures)
                gesture.Binding = new BulletproofBinding(gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider));

            foreach (var gesture in PanGestures)
                gesture.Binding = new BulletproofBinding(gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider));

            foreach (var gesture in PinchGestures)
                gesture.Binding = new BulletproofBinding(gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider));

            foreach (var gesture in RotateGestures)
                gesture.Binding = new BulletproofBinding(gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider));
        }
    }
}
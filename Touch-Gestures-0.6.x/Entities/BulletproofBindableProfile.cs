using OpenTabletDriver.Plugin;
using TouchGestures.Entities;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Lib.Entities
{
    public class BulletproofBindableProfile : BindableProfile
    {
        public override void ConstructBindings(SharedTabletReference? tablet = null)
        {
            if (tablet is not BulletproofSharedTabletReference bTablet)
                return;

            foreach (var gesture in TapGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider);

            foreach (var gesture in SwipeGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider);

            foreach (var gesture in HoldGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider);

            foreach (var gesture in PanGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider);

            foreach (var gesture in PinchGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider);

            foreach (var gesture in RotateGestures)
                gesture.Binding = gesture.Store?.Construct<IBinding>(bTablet?.ServiceProvider);
        }
    }
}
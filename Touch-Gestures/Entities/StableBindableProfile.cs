using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Entities
{
    public class StableBindableProfile : BindableProfile
    {
        public override void ConstructBindings(SharedTabletReference? tablet = null)
        {
            if (tablet == null)
                return;

            foreach (var gesture in TapGestures)
                gesture.Binding = new StableBinding(gesture.Store?.Construct<IBinding>());

            foreach (var gesture in SwipeGestures)
                gesture.Binding = new StableBinding(gesture.Store?.Construct<IBinding>());

            foreach (var gesture in HoldGestures)
                gesture.Binding = new StableBinding(gesture.Store?.Construct<IBinding>());

            foreach (var gesture in PanGestures)
                gesture.Binding = new StableBinding(gesture.Store?.Construct<IBinding>());

            foreach (var gesture in PinchGestures)
                gesture.Binding = new StableBinding(gesture.Store?.Construct<IBinding>());

            foreach (var gesture in RotateGestures)
                gesture.Binding = new StableBinding(gesture.Store?.Construct<IBinding>());
        }
    }
}
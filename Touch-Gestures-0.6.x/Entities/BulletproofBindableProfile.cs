using TouchGestures.Bindings;
using TouchGestures.Entities;
using TouchGestures.Extensions;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Lib.Entities
{
    public class BulletproofGestureProfile : GestureProfile
    {
        public override void ConstructBindings(SharedTabletReference tablet)
        {
            if (tablet is not BulletproofSharedTabletReference btablet) return;

            foreach (var gesture in this)
            {
                gesture.Binding = new BulletproofBinding(gesture.Store, btablet.ToState(), btablet.ServiceProvider);
                gesture.Binding?.Construct();
            }
        }
    }
}
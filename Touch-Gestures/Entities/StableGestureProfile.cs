using TouchGestures.Bindings;
using TouchGestures.Extensions;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Lib.Entities
{
    public class StableGestureProfile : GestureProfile
    {
        public override void ConstructBindings(SharedTabletReference tablet)
        {
            foreach (var gesture in this)
            {
                gesture.Binding = new StableBinding(gesture.Store, tablet.ToState());
                gesture.Binding?.Construct();
            }
        }
    }
}
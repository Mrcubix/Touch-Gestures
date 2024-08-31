using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Entities
{
    public class BulletproofBinding : ISharedBinding<IBinding>
    {
        public BulletproofBinding(IBinding? binding)
        {
            Binding = binding;
        }

        public IBinding? Binding { get; set; }
        public bool IsPressed { get; set; }

        public void Press()
        {
            if (Binding is IStateBinding stateBinding)
                stateBinding.Press(null!, null!);

            IsPressed = true;
        }

        public void Release()
        {
            if (Binding is IStateBinding stateBinding)
                stateBinding?.Release(null!, null!);

            IsPressed = false;
        }
    }
}
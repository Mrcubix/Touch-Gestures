using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Entities
{
    public class StableBinding : ISharedBinding<IBinding>
    {
        public StableBinding(IBinding? binding)
        {
            Binding = binding;
        }

        public IBinding? Binding { get; set; }
        public bool IsPressed { get; set; }

        public void Press()
        {
            Binding?.Press();

            IsPressed = true;
        }

        public void Release()
        {
            Binding?.Release();

            IsPressed = false;
        }
    }
}
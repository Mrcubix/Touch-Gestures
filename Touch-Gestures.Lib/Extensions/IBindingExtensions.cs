using OpenTabletDriver.Plugin;

namespace TouchGestures.Lib.Extensions
{
    public static class IBindingExtensions
    {
#if NET6_0
        public static void Press(this IBinding binding)
        {
            if (binding is IStateBinding stateBinding)
                stateBinding.Press();
        }

        public static void Release(this IBinding binding)
        {
            if (binding is IStateBinding stateBinding)
                stateBinding.Release();
        }
#endif
    }
}
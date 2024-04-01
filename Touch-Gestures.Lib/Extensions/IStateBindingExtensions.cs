using OpenTabletDriver.Plugin;

namespace TouchGestures.Lib.Extensions
{
    public static class IStateBindingExtensions
    {
#if NET6_0
        public static void Press(this IStateBinding binding)
        {
            binding?.Press(null!, null!);
        }

        public static void Release(this IStateBinding binding)
        {
            binding?.Release(null!, null!);
        }
#endif
    }
}
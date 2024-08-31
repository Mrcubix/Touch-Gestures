using System;

namespace TouchGestures.Lib.Reflection
{
    public interface IServiceManager : IServiceProvider
    {
        bool AddService<T>(Func<T> value);
        void ResetServices();
    }
}

namespace TouchGestures.Lib.Interfaces
{
    public interface ISharedBinding
    {
        bool IsPressed { get; protected set; }
        void Press();
        void Release();
    }

    public interface ISharedBinding<T> : ISharedBinding
    {
        T? Binding { get; set; }
    }
}
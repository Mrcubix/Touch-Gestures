namespace TouchGestures.Lib.Interfaces
{
    public interface ITouchesCountDependant
    {
        /// <summary>
        ///   The number of touches required to trigger the gesture.
        /// </summary>
        /// <remarks>
        ///   Changing this value while the gesture has started is not supported.
        /// </remarks>
        int RequiredTouchesCount { get; set; }
    }
}
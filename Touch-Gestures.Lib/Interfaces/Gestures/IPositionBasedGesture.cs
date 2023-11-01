using System.Numerics;

namespace TouchGestures.Lib.Interfaces
{
    /// <summary>
    ///    A gesture that may happen after moving a certain distance from the start position.
    /// </summary>
    public interface IPositionBasedGesture : IGesture
    {
        /// <summary>
        ///   The position where the gesture started.
        /// </summary>
        Vector2 StartPosition { get; }

        /// <summary>
        ///   The distance that must be traveled from the start position to trigger the gesture.
        /// </summary>
        Vector2 Threshold { get; }
    }
}
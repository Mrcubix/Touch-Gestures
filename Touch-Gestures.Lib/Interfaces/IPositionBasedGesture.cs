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
        public virtual Vector2 StartPosition
        {
            get => throw new System.NotImplementedException();
            protected set => throw new System.NotImplementedException();
        }

        /// <summary>
        ///   The distance that must be traveled from the start position to trigger the gesture.
        /// </summary>
        public virtual Vector2 Threshold
        {
            get => throw new System.NotImplementedException();
            protected set => throw new System.NotImplementedException();
        }
    }
}
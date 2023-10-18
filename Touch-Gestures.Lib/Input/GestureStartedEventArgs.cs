using System.Numerics;

namespace TouchGestures.Lib.Input
{
    public class GestureStartedEventArgs : GestureEventArgs
    {
        #region Constructors

        public GestureStartedEventArgs() : base()
        {
        }

        public GestureStartedEventArgs(bool hasStarted, bool hasEnded, bool hasCompleted, Vector2 startPosition) : base(hasStarted, hasEnded, hasCompleted)
        {
            StartPosition = startPosition;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The start position of the gesture.
        /// </summary>
        public Vector2 StartPosition { get; set; }

        #endregion
    }
}
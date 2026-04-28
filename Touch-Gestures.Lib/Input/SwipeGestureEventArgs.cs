using TouchGestures.Lib.Enums;

namespace TouchGestures.Lib.Input
{
    public class SwipeGestureEventArgs : GestureEventArgs
    {
        #region Constructors

        public SwipeGestureEventArgs() : base()
        {
        }

        public SwipeGestureEventArgs(bool hasStarted, bool hasActivated, bool hasEnded, bool hasCompleted, SwipeDirection direction) : base(hasStarted, hasActivated, hasEnded, hasCompleted)
        {
            Direction = direction;
        }

        public SwipeDirection Direction { get; set; }

        #endregion
    }
}
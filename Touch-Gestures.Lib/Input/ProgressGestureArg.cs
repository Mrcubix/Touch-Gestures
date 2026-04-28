namespace TouchGestures.Lib.Input
{
    public class ProgressGestureArg : GestureEventArgs
    {
        #region Constructors

        public ProgressGestureArg()
        {
            LastCompletedNodeIndex = -1;
        }

        public ProgressGestureArg(bool hasStarted, bool hasActivated, bool hasEnded, bool hasCompleted, int lastCompletedNodeIndex) : base(hasStarted, hasActivated, hasEnded, hasCompleted)
        {
            LastCompletedNodeIndex = lastCompletedNodeIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The index of the last completed node in the <see cref="INodeBasedGesture" />.
        /// </summary>
        public int LastCompletedNodeIndex { get; set; }

        #endregion
    }
}
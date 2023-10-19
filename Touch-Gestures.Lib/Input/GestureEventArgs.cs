namespace TouchGestures.Lib
{
    public class GestureEventArgs
    {
        #region Constructors
        
        public GestureEventArgs()
        {
            HasStarted = false;
            HasEnded = false;
            HasCompleted = false;
        }

        public GestureEventArgs(bool hasStarted, bool hasEnded, bool hasCompleted)
        {
            HasStarted = hasStarted;
            HasEnded = hasEnded;
            HasCompleted = hasCompleted;
        }

        #endregion

        #region Properties

        public bool HasStarted { get; set; }
        public bool HasEnded { get; set; }
        public bool HasCompleted { get; set; }

        #endregion
    }
}
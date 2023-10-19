using System;

namespace TouchGestures.Lib.Interfaces
{
    public interface ITimeBasedGesture : IGesture
    {
        /// <summary>
        ///   The time when the gesture started.
        /// </summary>
        public virtual DateTime TimeStarted
        {
            get => throw new NotImplementedException();
            protected set => throw new NotImplementedException();
        }

        /// <summary>
        ///   The time within which the gesture must be completed. <br/>
        ///   If the gesture is not completed within this time, it will be ended without being completed. <br/>
        ///   Measured in milliseconds.
        /// </summary>
        public virtual double Deadline
        {
            get => throw new NotImplementedException();
            protected set => throw new NotImplementedException();
        }
    }
}
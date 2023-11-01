using System;

namespace TouchGestures.Lib.Interfaces
{
    public interface ITimeBasedGesture : IGesture
    {
        /// <summary>
        ///   The time when the gesture started.
        /// </summary>
        DateTime TimeStarted { get; }

        /// <summary>
        ///   The time within which the gesture must be completed. <br/>
        ///   If the gesture is not completed within this time, it will be ended without being completed. <br/>
        ///   Measured in milliseconds.
        /// </summary>
        double Deadline { get; }
    }
}
using System;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Input;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{
    public abstract class TimeBasedGesture : ITimeBasedGesture
    {
        public virtual GestureType GestureType => GestureType.TimeBased;
        public event EventHandler<GestureStartedEventArgs> GestureStarted = null!;
        public event EventHandler<GestureEventArgs> GestureEnded = null!;
        public event EventHandler<GestureEventArgs> GestureCompleted = null!;
    }
}
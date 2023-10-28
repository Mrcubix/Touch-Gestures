using System;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Input;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{
    public abstract class PositionBasedGesture : ITimeBasedGesture
    {
        public virtual GestureType GestureType => GestureType.PositionBased;
        public event EventHandler<GestureStartedEventArgs> GestureStarted = null!;
        public event EventHandler<GestureEventArgs> GestureEnded = null!;
        public event EventHandler<GestureEventArgs> GestureCompleted = null!;
    }
}
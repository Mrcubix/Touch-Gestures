using System;
using TouchGestures.Lib.Entities.Gestures.Bases;

namespace TouchGestures.UX.Events;

public class GestureChangedEventArgs : EventArgs
{
    public Gesture? OldValue { get; }
    public Gesture? NewValue { get; }

    public GestureChangedEventArgs(Gesture? oldValue, Gesture? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
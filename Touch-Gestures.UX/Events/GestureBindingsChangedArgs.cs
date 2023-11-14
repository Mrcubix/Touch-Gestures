using System;
using OpenTabletDriver.External.Common.Serializables;

namespace TouchGestures.UX.Events;

public class GestureBindingsChangedArgs : EventArgs
{
    public SerializablePluginSettings? OldValue { get; }

    public SerializablePluginSettings? NewValue { get; }

    public GestureBindingsChangedArgs(SerializablePluginSettings? oldValue, SerializablePluginSettings? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
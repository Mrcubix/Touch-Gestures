using System;
using OpenTabletDriver.External.Common.Serializables;

namespace TouchGestures.UX.Events;

public class GestureBindingsChangedArgs : EventArgs
{
    public SerializablePluginSettingsStore? OldValue { get; }

    public SerializablePluginSettingsStore? NewValue { get; }

    public GestureBindingsChangedArgs(SerializablePluginSettingsStore? oldValue, SerializablePluginSettingsStore? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
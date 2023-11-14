using System;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.Events;

public class GestureAddedEventArgs : EventArgs
{
    public BindingDisplayViewModel BindingDisplay { get; }

    public Gesture? Gesture { get; }

    public GestureAddedEventArgs(GestureSetupViewModel setup)
    {
        BindingDisplay = setup.BindingDisplay;
        Gesture = setup.BuildGesture();
    }
}
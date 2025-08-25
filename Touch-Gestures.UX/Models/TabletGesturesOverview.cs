using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.UX.ViewModels;

namespace TouchGestures.UX.Models;

public partial class TabletGesturesOverview : ObservableObject, IDisposable
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private SharedTabletReference _reference = null!;

    [ObservableProperty]
    private GestureProfile _profile = null!;

    [ObservableProperty]
    private ObservableCollection<GestureBindingDisplayViewModel> _gestures = new();

    public TabletGesturesOverview()
    {
    }

    public TabletGesturesOverview(string name)
    {
        Name = name;
    }

    public TabletGesturesOverview(GestureProfile profile)
    {
        Name = profile.Name;
    }

    public TabletGesturesOverview(SharedTabletReference tablet)
    {
        Name = tablet.Name;
        Reference = tablet;
    }

    public TabletGesturesOverview(SharedTabletReference tablet, GestureProfile profile)
    {
        Name = profile.Name;
        Reference = tablet;
        Profile = profile;
    }

    public void Add(GestureBindingDisplayViewModel gesture)
    {
        Gestures.Add(gesture);
        Profile.Add(gesture.AssociatedGesture);
    }

    public void Remove(GestureBindingDisplayViewModel gesture)
    {
        Gestures.Remove(gesture);
        Profile.Remove(gesture.AssociatedGesture);
    }

    public void Dispose()
    {
        foreach (var gesture in Gestures)
            gesture.Dispose();

        Gestures.Clear();
    }

    public override string ToString()
    {
        return Name;
    }
}
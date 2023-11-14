using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

#nullable enable

public partial class GestureTileViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _gestureName = "[Insert Gesture Name Here]";

    [ObservableProperty]
    private string _description = "[Insert Gesture Description Here]";

    [ObservableProperty]
    private IImage? _icon = null;
    
    [ObservableProperty]
    private GestureSetupViewModel _associatedSetup = new();

    public event EventHandler? Selected;

    [RelayCommand]
    protected virtual void SelectGesture() => Selected?.Invoke(this, EventArgs.Empty);
}
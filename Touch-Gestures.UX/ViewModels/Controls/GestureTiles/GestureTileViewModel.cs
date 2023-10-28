using System.Windows.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using TouchGestures.UX.ViewModels;

namespace TouchGestures.UX.Controls.GestureTiles;

public partial class GestureTileViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _gestureName = "[Insert Gesture Name Here]";

    [ObservableProperty]
    private string _description = "[Insert Gesture Description Here]";

    [ObservableProperty]
    private IImage? _icon = null;

    [ObservableProperty]
    private ICommand? _command = null;
}
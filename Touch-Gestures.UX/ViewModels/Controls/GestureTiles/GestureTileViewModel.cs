using System.Windows.Input;
using Avalonia.Media;
using OpenTabletDriver.External.Avalonia.ViewModels;
using ReactiveUI;

namespace TouchGestures.UX.Controls.GestureTiles;

public class GestureTileViewModel : ViewModelBase
{
    private string _gestureName = "[Insert Gesture Name Here]";
    private string _description = "[Insert Gesture Description Here]";
    private IImage? _icon = null;
    private ICommand? _command = null;

    public string GestureName
    {
        get => _gestureName;
        set => this.RaiseAndSetIfChanged(ref _gestureName, value);
    }

    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    public IImage? Icon
    {
        get => _icon;
        set => this.RaiseAndSetIfChanged(ref _icon, value);
    }

    public ICommand? Command
    {
        get => _command;
        set => this.RaiseAndSetIfChanged(ref _command, value);
    }
}
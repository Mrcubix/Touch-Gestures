using CommunityToolkit.Mvvm.ComponentModel;

namespace TouchGestures.UX.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private NodeGestureEditorViewModel _nodeGestureEditorViewModel = new();

    [ObservableProperty]
    private GestureSelectionScreenViewModel _gestureSelectionScreenViewModel = new();

    public MainViewModel()
    {

    }
}

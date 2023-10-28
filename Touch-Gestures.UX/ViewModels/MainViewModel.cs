using ReactiveUI;

namespace TouchGestures.UX.ViewModels;

public class MainViewModel : ViewModelBase
{
    private NodeGestureEditorViewModel _nodeGestureEditorViewModel = new();
    private GestureSelectionScreenViewModel _gestureSelectionScreenViewModel = new();

    public NodeGestureEditorViewModel NodeGestureEditorViewModel
    {
        get => _nodeGestureEditorViewModel;
        set => this.RaiseAndSetIfChanged(ref _nodeGestureEditorViewModel, value);
    }

    public GestureSelectionScreenViewModel GestureSelectionScreenViewModel
    {
        get => _gestureSelectionScreenViewModel;
        set => this.RaiseAndSetIfChanged(ref _gestureSelectionScreenViewModel, value);
    }

    public MainViewModel()
    {

    }
}

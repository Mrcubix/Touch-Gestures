using System.Collections.ObjectModel;
using Avalonia;
using ReactiveUI;
using TouchGestures.UX.ViewModels.Controls.Nodes;

namespace TouchGestures.UX.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ObservableCollection<NodeViewModel> _nodes = new();

    public ObservableCollection<NodeViewModel> Nodes
    {
        get => _nodes;
        set => this.RaiseAndSetIfChanged(ref _nodes, value);
    }

    public MainViewModel()
    {
        var rect1 = new RectangleNodeViewModel
        {
            X = 100,
            Y = 100,
            Width = 75,
            Height = 75
        };

        var rect2 = new RectangleNodeViewModel
        {
            X = 200,
            Y = 200,
            Width = 50,
            Height = 50
        };

        var circle1 = new CircleNodeViewModel
        {
            X = 300,
            Y = 300,
            Width = 25,
            Height = 25
        };

        Nodes.Add(rect1);
        Nodes.Add(rect2);
        Nodes.Add(circle1);
    }
}

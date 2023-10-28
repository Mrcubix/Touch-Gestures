using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using TouchGestures.UX.ViewModels.Controls.Nodes;

namespace TouchGestures.UX.Controls.Containers;

public class NodeCanvas : TemplatedControl
{
    public static readonly StyledProperty<ObservableCollection<NodeViewModel>> NodesProperty =
        AvaloniaProperty.Register<NodeCanvas, ObservableCollection<NodeViewModel>>(nameof(Nodes));

    public ObservableCollection<NodeViewModel> Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }
}
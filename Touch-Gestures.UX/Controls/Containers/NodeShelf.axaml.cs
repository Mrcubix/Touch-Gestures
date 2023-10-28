using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using TouchGestures.UX.Interfaces.Nodes;

namespace TouchGestures.UX.Controls.Containers;

public class NodeShelf : TemplatedControl
{
    public static readonly StyledProperty<ObservableCollection<INodeBuilder>> NodeBuildersProperty =
        AvaloniaProperty.Register<NodeShelf, ObservableCollection<INodeBuilder>>(nameof(NodeBuilders));

    public ObservableCollection<INodeBuilder> NodeBuilders
    {
        get => GetValue(NodeBuildersProperty);
        set => SetValue(NodeBuildersProperty, value);
    }
}
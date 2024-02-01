using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using TouchGestures.UX.ViewModels.Controls.Nodes;

namespace TouchGestures.UX.Controls.Containers;

public class NodeCanvas : TemplatedControl
{
    public static readonly StyledProperty<ObservableCollection<NodeViewModel>> NodesProperty =
        AvaloniaProperty.Register<NodeCanvas, ObservableCollection<NodeViewModel>>(nameof(Nodes));

    public static readonly StyledProperty<Size> MaxBoundsProperty =
        AvaloniaProperty.Register<NodeCanvas, Size>(nameof(MaxBounds), new Size(223.52, 139.7));

    public ObservableCollection<NodeViewModel> Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    public Size MaxBounds
    {
        get => GetValue(MaxBoundsProperty);
        set => SetValue(MaxBoundsProperty, value);
    }

    protected override Size ArrangeOverride(Size maxSize)
    {
        // Do scaling here
        var scaledWidth = maxSize.Height / MaxBounds.Height * MaxBounds.Width;
        var scaledHeight = maxSize.Width / MaxBounds.Width * MaxBounds.Height;

        if (scaledWidth > maxSize.Width)
        {
            Width = maxSize.Width;
            Height = scaledHeight;
        }
        else
        {
            Width = scaledWidth;
            Height = maxSize.Height;
        }

        return base.ArrangeOverride(maxSize);
    }
}
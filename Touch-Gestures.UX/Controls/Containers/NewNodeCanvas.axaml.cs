
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using TouchGestures.UX.ViewModels.Controls.Nodes;

namespace TouchGestures.UX.Controls.Containers;

#nullable enable

public partial class NewNodeCanvas : UserControl
{
    private Rect _baseBounds = default;

    public static readonly StyledProperty<ObservableCollection<NodeViewModel>> NodesProperty =
        AvaloniaProperty.Register<NewNodeCanvas, ObservableCollection<NodeViewModel>>(nameof(Nodes));

    public ObservableCollection<NodeViewModel> Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    public NewNodeCanvas()
    {
        //InitializeComponent();
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        if (_baseBounds == default)
            _baseBounds = Bounds;

        base.OnSizeChanged(e);
    }

    protected override void OnDataContextBeginUpdate()
    {
        base.OnDataContextBeginUpdate();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == NodesProperty)
        {
            if (change.OldValue is ObservableCollection<NodeViewModel> oldNodes)
            {
                oldNodes.CollectionChanged -= OnNodeCollectionChanged;
            }

            if (change.NewValue is ObservableCollection<NodeViewModel> newNodes)
            {
                newNodes.CollectionChanged += OnNodeCollectionChanged;
            }
        }

        base.OnPropertyChanged(change);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return base.ArrangeOverride(finalSize);
    }

    private void OnNodeCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:

                // Add the items to the canvas
                var itemsAdded = e.NewItems;

                break;
            case NotifyCollectionChangedAction.Remove:

                // Remove the items from the canvas
                var itemsRemoved = e.OldItems;

                break;
            case NotifyCollectionChangedAction.Replace:

                // Rebuild the canvas

                break;
            case NotifyCollectionChangedAction.Reset:

                // Rebuild the canvas

                break;
        }
    }
}
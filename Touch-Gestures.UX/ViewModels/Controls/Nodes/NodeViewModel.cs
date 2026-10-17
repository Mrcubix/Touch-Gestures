using TouchGestures.Lib.Interfaces;
using TouchGestures.Lib.Enums;
using Avalonia;
using TouchGestures.UX.Workarounds.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TouchGestures.UX.ViewModels.Controls.Nodes;

public abstract partial class NodeViewModel : ViewModelBase, INodeViewModel
{
    [ObservableProperty]
    private int _index;

    private GestureNodeShape _shape;

    [ObservableProperty]
    private bool _isGestureStart;

    [ObservableProperty]
    private bool _isGestureEnd;

    //[ObservableProperty]
    //private Point _position;

    //[ObservableProperty]
    //private Size _size;

    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private double _width;

    [ObservableProperty]
    private double _height;

    [ObservableProperty]
    private float _timestamp;

    [ObservableProperty]
    private float _timestampTolerance;

    [ObservableProperty]
    private bool _isHold;

    [ObservableProperty]
    private float _holdDuration;

    public NodeViewModel()
    {
    }

    public NodeViewModel(IGestureNode node)
    {
        Index = node.Index;
        Shape = node.Shape;

        IsGestureStart = node.IsGestureStart;
        IsGestureEnd = node.IsGestureEnd;

        /*
        Position = new Point(node.Position.X, node.Position.Y);
        Size = new Size(node.Size.X, node.Size.Y);
        */

        X = node.Position.X;
        Y = node.Position.Y;
        Width = node.Size.X;
        Height = node.Size.Y;

        Timestamp = node.Timestamp;
        TimestampTolerance = node.TimestampTolerance;

        IsHold = node.IsHold;
        HoldDuration = node.HoldDuration;
    }

    public GestureNodeShape Shape
    {
        get => _shape;
        init => SetProperty(ref _shape, value);
    }

    public abstract bool IsWithinNode(Point position, float timestamp);
}
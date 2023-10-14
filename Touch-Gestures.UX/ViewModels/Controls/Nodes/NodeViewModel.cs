using TouchGestures.Lib.Interfaces;
using ReactiveUI;
using TouchGestures.Lib.Enums;
using Avalonia;
using TouchGestures.UX.Workarounds.Interfaces;

namespace TouchGestures.UX.ViewModels.Controls.Nodes;

public abstract class NodeViewModel : ViewModelBase, INodeViewModel
{
    private int _index;
    private GestureNodeShape _shape;
    private bool _isGestureStart;
    private bool _isGestureEnd;
    /*
    private Point _position;
    private Size _size;
    */
    private double _x;
    private double _y;
    private double _width;
    private double _height;
    private float _timestamp;
    private float _timestampTolerance;
    private bool _isHold;
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

    public int Index
    {
        get => _index;
        set => this.RaiseAndSetIfChanged(ref _index, value);
    }

    public GestureNodeShape Shape
    {
        get => _shape;
        init => this.RaiseAndSetIfChanged(ref _shape, value);
    }

    public bool IsGestureStart
    {
        get => _isGestureStart;
        set => this.RaiseAndSetIfChanged(ref _isGestureStart, value);
    }

    public bool IsGestureEnd
    {
        get => _isGestureEnd;
        set => this.RaiseAndSetIfChanged(ref _isGestureEnd, value);
    }
    /*
    public Point Position
    {
        get => _position;
        set => this.RaiseAndSetIfChanged(ref _position, value);
    }

    public Size Size
    {
        get => _size;
        set => this.RaiseAndSetIfChanged(ref _size, value);
    }
    */
    public double X
    {
        get => _x;
        set => this.RaiseAndSetIfChanged(ref _x, value);
    }

    public double Y
    {
        get => _y;
        set => this.RaiseAndSetIfChanged(ref _y, value);
    }

    public double Width
    {
        get => _width;
        set => this.RaiseAndSetIfChanged(ref _width, value);
    }

    public double Height
    {
        get => _height;
        set => this.RaiseAndSetIfChanged(ref _height, value);
    }

    public float Timestamp
    {
        get => _timestamp;
        set => this.RaiseAndSetIfChanged(ref _timestamp, value);
    }

    public float TimestampTolerance
    {
        get => _timestampTolerance;
        set => this.RaiseAndSetIfChanged(ref _timestampTolerance, value);
    }

    public bool IsHold
    {
        get => _isHold;
        set => this.RaiseAndSetIfChanged(ref _isHold, value);
    }

    public float HoldDuration
    {
        get => _holdDuration;
        set => this.RaiseAndSetIfChanged(ref _holdDuration, value);
    }
    
    public abstract bool IsWithinNode(Point position, float timestamp);
}
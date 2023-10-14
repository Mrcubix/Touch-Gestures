using System;
using System.Numerics;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Entities.Nodes;
using Avalonia;

namespace TouchGestures.UX.ViewModels.Controls.Nodes;

public class CircleNodeViewModel : NodeViewModel
{
    public CircleNodeViewModel()
    {
        Shape = GestureNodeShape.Circle;
    }

    public CircleNodeViewModel(Circle node) : base(node)
    {
        Shape = GestureNodeShape.Circle;
    }

    #region Interface Implementations

    public override bool IsWithinNode(Point position, float timestamp)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Casts

    // cast from Circle to CircleNodeViewModel 
    public static explicit operator CircleNodeViewModel(Circle node)
    {
        return new CircleNodeViewModel(node);
    }

    // cast from CircleNodeViewModel to Circle
    public static explicit operator Circle(CircleNodeViewModel viewModel)
    {
        return new Circle(new Vector2((float)viewModel.X, (float)viewModel.Y),
                          new Vector2((float)viewModel.Width, (float)viewModel.Height))
        {
            Index = viewModel.Index,
            Shape = GestureNodeShape.Circle,

            IsGestureStart = viewModel.IsGestureStart,
            IsGestureEnd = viewModel.IsGestureEnd,

            Timestamp = viewModel.Timestamp,
            TimestampTolerance = viewModel.TimestampTolerance,
            
            IsHold = viewModel.IsHold,
            HoldDuration = viewModel.HoldDuration
        };
    }

    #endregion
}
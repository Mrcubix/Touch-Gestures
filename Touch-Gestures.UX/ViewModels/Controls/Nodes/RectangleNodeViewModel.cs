using System;
using System.Numerics;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Entities.Nodes;
using Avalonia;

namespace TouchGestures.UX.ViewModels.Controls.Nodes;

public class RectangleNodeViewModel : NodeViewModel
{
    public RectangleNodeViewModel()
    {
        Shape = GestureNodeShape.Rectangle;
    }

    public RectangleNodeViewModel(Rectangle node) : base(node)
    {
        Shape = GestureNodeShape.Rectangle;
    }

    #region Interface Implementations

    public override bool IsWithinNode(Point position, float timestamp)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Casts

    // cast from Rectangle to RectangleNodeViewModel 
    public static explicit operator RectangleNodeViewModel(Rectangle node)
    {
        return new RectangleNodeViewModel(node);
    }

    // cast from RectangleNodeViewModel to Rectangle
    public static explicit operator Rectangle(RectangleNodeViewModel viewModel)
    {
        return new Rectangle(new Vector2((float)viewModel.X, (float)viewModel.Y),
                             new Vector2((float)viewModel.Width, (float)viewModel.Height))
        {
            Index = viewModel.Index,
            Shape = GestureNodeShape.Rectangle,

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
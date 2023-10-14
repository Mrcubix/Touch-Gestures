using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using DynamicData.Binding;
using TouchGestures.UX.ViewModels.Controls.Nodes;

namespace TouchGestures.UX.Controls.Nodes;

/// <summary>
///   A Control that can be dragged around by the user.
/// </summary>
/// <remarks>
///   Based on <see href="https://github.com/Abdesol/MovableControl">Abdesol/MovableControl</see>.
/// </remarks>
public abstract class DraggableNode : Shape
{
    /// <summary>
    ///   Static constructor to mark certain properties as affecting the geometry.
    /// </summary>
    /// <remarks>
    ///   This is required to be able to render the shape, at all.
    /// </remarks>
    static DraggableNode()
    {
        AffectsGeometry<DraggableNode>(BoundsProperty, StrokeThicknessProperty);
    }

    /// <summary>
    ///   Whether or not the Control is currently being dragged.
    /// </summary>
    private bool _isDragged = false;

    /// <summary>
    ///    A Control will not always be dragged from its position value, but is within its Size.
    /// </summary>
    private Point _dragStartPoint;

    /// <summary>
    ///   The parent of the Control, if it is a ContentPresenter.
    ///   This is used to get the actual parent of the Control.
    /// </summary>

    /// <summary>
    ///   The parent used to move the Control around from.
    /// </summary>
    private Control? _realParent = null!;

    /// <summary>
    ///   The parent of the Control.
    /// </summary>
    /// <remarks>
    ///   Must be a Control
    /// </remarks>
    private Control? _parentControl = null!;

    /// <summary>
    ///   Whether or not the parent of the Control is a ContentPresenter.
    /// </summary>
    private bool _isParentContentPresenter = false;

    private TranslateTransform _transform = null!;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ParentProperty)
        {
            if (Parent is ContentPresenter contentPresenter)
            {
                _realParent = contentPresenter.Parent as Control;
                _isParentContentPresenter = _realParent != null;
            }
            else if (Parent is Control parent)
                _realParent = parent;

            // we cache the parent to avoid having to typecheck & cast it every time
            _parentControl = Parent as Control;
        }

        base.OnPropertyChanged(change);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _isDragged = true;

        if (_parentControl == null)
            return;

        _dragStartPoint = e.GetPosition(_parentControl);

        if (_transform != null)
        {
            _dragStartPoint = new Point(_dragStartPoint.X - _transform.X, 
                                        _dragStartPoint.Y - _transform.Y);
        }

        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        _isDragged = false;

        base.OnPointerReleased(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (!_isDragged)
            return;

        if (_realParent == null)
            return;

        Point currentPos = e.GetPosition(_realParent);

        var parentBounds = _realParent.Bounds;

        var offsetX = currentPos.X - _dragStartPoint.X;
        var offsetY = currentPos.Y - _dragStartPoint.Y;

        // clamp to parent bounds
        offsetX = Math.Clamp(offsetX, 0, parentBounds.Width - Width);
        offsetY = Math.Clamp(offsetY, 0, parentBounds.Height - Height);

        // Canvas.Left & Canvas.Top need to be set somwhere
        if (_isParentContentPresenter && DataContext is NodeViewModel nodeViewModel)
        {
            nodeViewModel.X = offsetX;
            nodeViewModel.Y = offsetY;
        }
        else
        {
            _transform = new TranslateTransform(offsetX, offsetY);
            RenderTransform = _transform;
        }

        base.OnPointerMoved(e);
    }
}

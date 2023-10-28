using Avalonia;
using Avalonia.Media;

namespace TouchGestures.UX.Controls.Nodes;

#nullable enable

public class RectangleNode : DraggableNode
{
    static RectangleNode()
    {
        AffectsGeometry<RectangleNode>(BoundsProperty, StrokeThicknessProperty);
    }

    protected override Geometry? CreateDefiningGeometry()
    {
        var rect = new Rect(Bounds.Size).Deflate(StrokeThickness / 2);
        return new RectangleGeometry(rect);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(StrokeThickness, StrokeThickness);
    }
}
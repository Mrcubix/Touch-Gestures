using Avalonia;
using Avalonia.Media;

namespace TouchGestures.UX.Controls.Nodes;

public class CircleNode : DraggableNode
{
    static CircleNode()
    {
        AffectsGeometry<CircleNode>(BoundsProperty, StrokeThicknessProperty);
    }

    protected override Geometry? CreateDefiningGeometry()
    {
        var rect = new Rect(Bounds.Size).Deflate(StrokeThickness / 2);
        return new EllipseGeometry(rect);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(StrokeThickness, StrokeThickness);
    }
}
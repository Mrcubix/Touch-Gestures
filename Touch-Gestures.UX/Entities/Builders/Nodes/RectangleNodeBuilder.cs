using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using TouchGestures.UX.Interfaces.Nodes;
using TouchGestures.UX.ViewModels.Controls.Nodes;

namespace TouchGestures.UX.Entities.Builders.Nodes;

#nullable enable

public class RectangleNodeBuilder : INodeBuilder
{
    #region Constructors

    static RectangleNodeBuilder()
    {
        RenderedIcon = new RenderTargetBitmap(new PixelSize(256, 256));
        BuildIcon();
    }

    #endregion

    #region Interface Implementation

    public event EventHandler? BuildRequested;

    public string Name { get; init; } = "Rectangle";
    public IImage Icon { get; init; } = RenderedIcon;

    public NodeViewModel Build() => new RectangleNodeViewModel();

    public void RequestBuild()
    {
        BuildRequested?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        RenderedIcon?.Dispose();
    }

    #endregion

    #region Static Elements

    internal static RenderTargetBitmap RenderedIcon { get; set; }

    internal static void BuildIcon()
    {
        using DrawingContext context = RenderedIcon.CreateDrawingContext();

        var rect = new Rect(0, 0, 256, 256);

        var brush = SolidColorBrush.Parse("#6C877E");

        context.DrawRectangle(brush, null, rect);
    }

    #endregion
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace TouchGestures.UX.Controls.Enhancements;

public class EnhancedGridSplitter : GridSplitter
{
    // styledproperty for the Background rectangle fill
    public static readonly StyledProperty<IBrush> BackgroundFillProperty =
        AvaloniaProperty.Register<EnhancedGridSplitter, IBrush>(nameof(BackgroundFill), Brushes.Black);

    public static readonly StyledProperty<string> DragIndicatorGlyphProperty =
        AvaloniaProperty.Register<EnhancedGridSplitter, string>(nameof(DragIndicatorGlyph), "⁞");

    public IBrush BackgroundFill
    {
        get => GetValue(BackgroundFillProperty);
        set => SetValue(BackgroundFillProperty, value);
    }

    public string DragIndicatorGlyph
    {
        get => GetValue(DragIndicatorGlyphProperty);
        set => SetValue(DragIndicatorGlyphProperty, value);
    }
}